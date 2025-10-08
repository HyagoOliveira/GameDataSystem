using System;
using System.Linq;
using System.Collections;
using ActionCode.Persistence;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Provides an abstract base class for managing game data, including local and cloud-based CRUD (CReate, Update, Delete) operations.
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

        [Header("Slots")]
        [SerializeField, Tooltip("The total number of available slots.")]
        private int availableSlots = 4;
        [Tooltip("The Save Slot name to use.")]
        public string slotName = "SaveSlot";

        public event Action OnSaveDataStarted;
        public event Action OnSaveDataFinished;

        /// <summary>
        /// The Game main data.
        /// </summary>
        public T Data => gameData;

        public int AvailableSlots => availableSlots;

        public ICloudProvider CloudProvider => cloudProvider.Value;

        public int LastSlotIndex
        {
            //TODO refact PersistenceSettings, removing lastSlotKey
            get => PlayerPrefs.GetInt(LAST_SLOT_INDEX_KEY, 0);
            private set => PlayerPrefs.SetInt(LAST_SLOT_INDEX_KEY, value);
        }

        public const string LAST_SLOT_INDEX_KEY = "LastSlotIndex";

        private readonly Lazy<ICloudProvider> cloudProvider = new(GetavailableCloudProvider);

        private void OnValidate() => availableSlots = Mathf.Clamp(availableSlots, 1, 16);

        public string GetSlotName(int index) => $"{slotName}-{index:D2}";
        public string GetSerializationExtension() => SerializerFactory.Create(persistence.serializer).Extension;

        public bool HasCloudProvider() => false;// CloudProvider != null;

        public void LoadData(object data)
        {
            var gameData = data as T;
            var serializer = persistence.GetFileSystem().Serializer;
            var content = serializer.Serialize(gameData);

            serializer.Deserialize(content, ref this.gameData);
            LastSlotIndex = this.gameData.SlotIndex;
        }

        public async Awaitable SaveDataAsync() => await SaveDataAsync(LastSlotIndex);

        public async Awaitable SaveDataAsync(int slot)
        {
            OnSaveDataStarted?.Invoke();
            gameData.UpdateData(slot);

            await SaveLocallyAsync(Data, slot);

            if (HasCloudProvider())
            {
                var name = GetSlotName(slot);
                await CloudProvider.SaveAsync(Data, name);
            }
            OnSaveDataFinished?.Invoke();
        }

        public async Awaitable LoadFromLastSlotAsync() => await LoadLocallyAsync(LastSlotIndex);
        public async Awaitable LoadLocallyAsync(int slot) => await persistence.TryLoadAsync(Data, GetSlotName(slot));

        public async Awaitable LoadRemotelyAsync(int slot)
        {
            var name = GetSlotName(slot);
            var json = await CloudProvider?.LoadAsync(name);
            var hasJson = !string.IsNullOrEmpty(json);
            if (hasJson) persistence.GetFileSystem().Serializer.Deserialize(json, ref gameData);
        }

        public async Awaitable<T> GetLocalDataAsync(int slot)
        {
            var data = CreateInstance<T>();
            var name = GetSlotName(slot);
            var wasLoaded = await persistence.TryLoadAsync(data, name);
            return wasLoaded ? data : null;
        }

        public async Awaitable<bool> IsContinueAvailable() => await HasLastSlotAvailable();

        public async Awaitable DeleteAsync(int slot)
        {
            await Awaitable.EndOfFrameAsync();
            //await CloudProvider?.DeleteAsync(persistenceSettings.GetSlotName(slot));
            DeleteLocally(slot);
        }

        public async Awaitable DeleteAllAsync()
        {
            DeleteAllLocally();
            await CloudProvider?.DeleteAllAsync();
        }

        public async Awaitable<IList> LoadAllLocallyAsync()
        {
            var names = persistence.GetNames();
            var slots = new T[names.Count()];

            for (var i = 0; i < slots.Length; i++)
            {
                var slotName = names.ElementAt(i);
                slots[i] = CreateInstance<T>();
                await persistence.TryLoadAsync(slots[i], slotName);
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

        protected virtual async Awaitable SaveLocallyAsync(ScriptableObject data, int slot)
        {
            LastSlotIndex = slot;
            await persistence.SaveAsync(data, GetSlotName(slot));
        }

        protected virtual void DeleteAllLocally() => persistence.DeleteAll();
        protected virtual void DeleteLocally(int slot) => persistence.Delete(GetSlotName(slot));

        private async Awaitable<bool> HasLastSlotAvailable()
        {
            var data = CreateInstance<T>();
            var name = GetSlotName(LastSlotIndex);
            return await persistence.TryLoadAsync(data, name);
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