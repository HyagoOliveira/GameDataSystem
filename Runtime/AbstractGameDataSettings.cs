using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    public abstract class AbstractGameDataSettings<T> : ScriptableObject, IDataModel where T : AbstractGameData
    {
        [SerializeField] private T gameData;
        [SerializeField] private PersistenceSettings persistenceSettings;

        /// <summary>
        /// The Game main data.
        /// </summary>
        public T Data => gameData;

        /// <summary>
        /// The serialization settings.
        /// </summary>
        public PersistenceSettings Serialization => persistenceSettings;

        public ICloudProvider CloudProvider => cloudProvider.Value;

        private readonly Lazy<ICloudProvider> cloudProvider = new(GetavailableCloudProvider);

        public bool HasCloudProvider() => CloudProvider != null;

        public void UpdateData(object data)
        {
            var gameData = data as T;
            var serializer = persistenceSettings.GetFileSystem().Serializer;
            var content = serializer.Serialize(gameData);

            serializer.Deserialize(content, ref this.gameData);
        }

        public async void SaveData(int slot)
        {
            gameData.UpdateData(slot);

            await SaveLocallyAsync(Data, slot);

            if (HasCloudProvider())
            {
                var name = persistenceSettings.GetSlotName(slot);
                await CloudProvider.SaveAsync(Data, name);
            }
        }

        public async Awaitable LoadFromLastSlotAsync() => await persistenceSettings.TryLoadLastSlot(Data);
        public async Awaitable LoadLocallyAsync(int slot) => await persistenceSettings.TryLoad(Data, slot);

        public async Awaitable LoadRemotelyAsync(int slot)
        {
            var name = persistenceSettings.GetSlotName(slot);
            var json = await CloudProvider?.LoadAsync(name);
            var hasJson = !string.IsNullOrEmpty(json);
            if (hasJson) persistenceSettings.GetFileSystem().Serializer.Deserialize(json, ref gameData);
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
            var names = persistenceSettings.GetNames();
            var slots = new T[names.Count()];

            for (var i = 0; i < slots.Length; i++)
            {
                var slotName = names.ElementAt(i);
                slots[i] = CreateInstance<T>();
                await persistenceSettings.TryLoad(slots[i], slotName);
            }

            return slots;
        }

        public async Awaitable<IList> LoadAllRemotelyAsync(string playerId)
        {
            var remoteData = await CloudProvider?.LoadAllAsync(playerId);
            var gamesData = new T[remoteData.Count()];
            var serializer = persistenceSettings.GetFileSystem().Serializer;

            for (var i = 0; i < gamesData.Length; i++)
            {
                var content = remoteData.ElementAt(i);
                gamesData[i] = CreateInstance<T>();
                serializer.Deserialize(content, ref gamesData[i]);
            }

            return gamesData;
        }

        protected virtual async Awaitable SaveLocallyAsync(ScriptableObject data, int slot) =>
            await persistenceSettings.Save(data, slot);

        protected virtual bool TryDeleteLocally(int slot) => persistenceSettings.TryDelete(slot);
        protected virtual bool TryDeleteAllLocally() => persistenceSettings.TryDeleteAll();

        private async Awaitable<bool> HasLastSlotAvailable()
        {
            var data = CreateInstance<T>();
            return await persistenceSettings.TryLoadLastSlot(data);
        }

        // The only available Cloud provider for now is from Unity Service.
        // No need to have others for now.
        private static ICloudProvider GetavailableCloudProvider()
        {
#if UNITY_CLOUD_SAVE
            return new UnityCloudService();
#endif
            return null;
        }
    }
}