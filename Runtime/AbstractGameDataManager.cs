using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Provides an abstract base class for managing game data, including local and cloud-based persistence.
    /// </summary>
    /// <remarks>
    /// This class handles operations such as saving, loading, and deleting game data both locally
    /// and remotely. It also provides support for cloud-based persistence if a cloud provider is available. Derived
    /// classes can override certain methods to customize local persistence behavior.
    /// </remarks>
    /// <typeparam name="T">The type of game data managed by this class. Must derive from <see cref="AbstractGameData"/>.</typeparam>
    public abstract class AbstractGameDataManager<T> : ScriptableObject, IGameDataManager where T : AbstractGameData
    {
        [SerializeField] private T gameData;
        [SerializeField] private PersistenceSettings persistence;
        [SerializeField, Tooltip("The total number of available slots.")]
        private int availableSlots = 4;

        /// <summary>
        /// The Game main data.
        /// </summary>
        public T Data => gameData;

        public int AvailableSlots => availableSlots;

        public ICloudProvider CloudProvider => cloudProvider.Value;

        private readonly Lazy<ICloudProvider> cloudProvider = new(GetavailableCloudProvider);

        private void OnValidate() => availableSlots = Mathf.Clamp(availableSlots, 1, 16);

        public bool HasCloudProvider() => CloudProvider != null;

        public void UpdateData(object data)
        {
            var gameData = data as T;
            var serializer = persistence.GetFileSystem().Serializer;
            var content = serializer.Serialize(gameData);

            serializer.Deserialize(content, ref this.gameData);
        }

        public async void SaveData(int slot)
        {
            gameData.UpdateData(slot);

            await SaveLocallyAsync(Data, slot);

            if (HasCloudProvider())
            {
                var name = persistence.GetSlotName(slot);
                await CloudProvider.SaveAsync(Data, name);
            }
        }

        public async Awaitable LoadFromLastSlotAsync() => await persistence.TryLoadLastSlot(Data);
        public async Awaitable LoadLocallyAsync(int slot) => await persistence.TryLoad(Data, slot);

        public async Awaitable LoadRemotelyAsync(int slot)
        {
            var name = persistence.GetSlotName(slot);
            var json = await CloudProvider?.LoadAsync(name);
            var hasJson = !string.IsNullOrEmpty(json);
            if (hasJson) persistence.GetFileSystem().Serializer.Deserialize(json, ref gameData);
        }

        public async Awaitable<bool> IsContinueAvailable() => await HasLastSlotAvailable();

        public async Awaitable<bool> TryDeleteAsync(int slot)
        {
            await Awaitable.EndOfFrameAsync();
            //await CloudProvider?.DeleteAsync(persistenceSettings.GetSlotName(slot));
            return TryDeleteLocally(slot);
        }

        public async Awaitable<bool> TryDeleteAllAsync() =>
            TryDeleteAllLocally() &&
            await CloudProvider?.DeleteAllAsync();

        public async Awaitable<IList> ListSlotsAsync()
        {
            var names = persistence.GetNames();
            var slots = new T[names.Count()];

            for (var i = 0; i < slots.Length; i++)
            {
                var slotName = names.ElementAt(i);
                slots[i] = CreateInstance<T>();
                await persistence.TryLoad(slots[i], slotName);
            }

            return slots;
        }

        public async Awaitable<IList> LoadAllRemotelyAsync(string playerId)
        {
            var remoteData = await CloudProvider?.LoadAllAsync(playerId);
            var gamesData = new T[remoteData.Count()];
            var serializer = persistence.GetFileSystem().Serializer;

            for (var i = 0; i < gamesData.Length; i++)
            {
                var content = remoteData.ElementAt(i);
                gamesData[i] = CreateInstance<T>();
                serializer.Deserialize(content, ref gamesData[i]);
            }

            return gamesData;
        }

        protected virtual async Awaitable SaveLocallyAsync(ScriptableObject data, int slot) =>
            await persistence.Save(data, slot);

        protected virtual bool TryDeleteLocally(int slot) => persistence.TryDelete(slot);
        protected virtual bool TryDeleteAllLocally() => persistence.TryDeleteAll();

        private async Awaitable<bool> HasLastSlotAvailable()
        {
            var data = CreateInstance<T>();
            return await persistence.TryLoadLastSlot(data);
        }

        // The only available Cloud provider for now is from Unity Service.
        // No need to have others for now.
        private static ICloudProvider GetavailableCloudProvider()
        {
#if UNITY_CLOUD_SAVE
            return new UnityCloudService();
#else
            return null;
#endif
        }

    }
}