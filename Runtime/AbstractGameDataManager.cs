using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Provides an abstract base class for managing game data, including local and cloud-based CRUD (CReate, Update, Delete) operations.
    /// </summary>
    /// <remarks>
    /// This class handles operations such as saving, loading, and deleting game data both locally and remotely. 
    /// It also provides support for cloud-based persistence if a cloud provider is available. 
    /// </remarks>
    /// <typeparam name="T">The type of Game Data managed by this class. Must derive from <see cref="AbstractGameData"/>.</typeparam>
    public abstract class AbstractGameDataManager<T> : ScriptableObject, IGameDataManager where T : AbstractGameData
    {
        [SerializeField] private T gameData;
        [SerializeField] private PersistenceSettings persistence;

        [Header("Slots")]
        [SerializeField, Tooltip("The total number of available slots.")]
        private int availableSlots = 4;
        [Tooltip("The Save Slot name to use.")]
        public string slotName = "SaveSlot";

        /// <summary>
        /// Action fired when the save process starts.
        /// </summary>
        public event Action OnSaveStarted;

        /// <summary>
        /// Action fired when the save process finishes.
        /// </summary>
        public event Action OnSaveFinished;

        /// <summary>
        /// The Game main data.
        /// </summary>
        public T Data => gameData;
        public int AvailableSlots => availableSlots;
        public PersistenceSettings Persistence => persistence;
        public ICloudProvider CloudProvider => cloudProvider.Value;

        /// <summary>
        /// The last slot index used to save or load data.
        /// </summary>
        public int LastSlotIndex
        {
            get => PlayerPrefs.GetInt(LAST_SLOT_INDEX_KEY, defaultValue: 0);
            private set => PlayerPrefs.SetInt(LAST_SLOT_INDEX_KEY, value);
        }

        public const int MAX_SLOTS = 16;
        public const string LAST_SLOT_INDEX_KEY = "LastSlotIndex";

        private readonly Lazy<ICloudProvider> cloudProvider = new(GetavailableCloudProvider);

        private void OnValidate() => availableSlots = Mathf.Clamp(availableSlots, 1, MAX_SLOTS);

        public string GetSlotName(int slot) => $"{slotName}-{slot:D2}";
        public string GetSerializedExtension() => SerializerFactory.Create(Persistence.serializer).Extension;

        public async Awaitable<bool> IsContinueAvailable() => await HasLastSlotAvailable();

        public async Awaitable<bool> HasLastSlotAvailable()
        {
            var data = CreateInstance<T>();
            var name = GetSlotName(LastSlotIndex);
            var useRawFile = ShouldLoadFromRawFile();
            return await Persistence.TryLoadAsync(data, name, useRawFile);
        }

        public async Awaitable<T> GetDataAsync(int slot)
        {
            var data = CreateInstance<T>();
            var name = GetSlotName(slot);
            var useRawFile = ShouldLoadFromRawFile();
            var wasLoaded = await Persistence.TryLoadAsync(data, name, useRawFile);
            return wasLoaded ? data : null;
        }

        public async Awaitable SaveAsync() => await SaveAsync(LastSlotIndex);

        public async Awaitable SaveAsync(int slot)
        {
            OnSaveStarted?.Invoke();
            gameData.UpdateData(slot);

            LastSlotIndex = slot;
            var name = GetSlotName(slot);

            await Persistence.SaveAsync(Data, name);
            if (HasCloudProvider()) await CloudProvider.SaveAsync(Data, name);

            OnSaveFinished?.Invoke();
        }

        public async Awaitable<bool> TryLoadFromLastSlotAsync() => await TryLoadAsync(LastSlotIndex);
        public async Awaitable<bool> TryLoadAsync(string path) => await Persistence.TryLoadAsync(Data, path);
        public async Awaitable<bool> TryLoadAsync(int slot)
        {
            var name = GetSlotName(slot);
            var useRawFile = ShouldLoadFromRawFile();
            return await Persistence.TryLoadAsync(Data, name, useRawFile);
        }

        public async Awaitable<IList> LoadAllAsync()
        {
            var names = Persistence.GetNames();
            var slots = new T[names.Count()];

            for (var i = 0; i < slots.Length; i++)
            {
                var slotName = names.ElementAt(i);
                slots[i] = CreateInstance<T>();
                await Persistence.TryLoadAsync(slots[i], slotName);
            }

            return slots;
        }

        public async Awaitable DeleteAsync(int slot)
        {
            var name = GetSlotName(slot);
            if (HasCloudProvider()) await CloudProvider?.DeleteAsync(name);
            Persistence.Delete(name);
        }

        public async Awaitable DeleteAllAsync()
        {
            Persistence.DeleteAll();
            await CloudProvider?.DeleteAllAsync();
        }

        /// <summary>
        /// Raw file is human legible file (the pretty .json)
        /// </summary>
        /// <returns>Whether should load from the Raw File.</returns>
        public static bool ShouldLoadFromRawFile() => Debug.isDebugBuild;

        //TODO improve remote functions
        #region REMOTE
        public bool HasCloudProvider() => false;// CloudProvider != null;

        public async Awaitable LoadRemotelyAsync(int slot)
        {
            var name = GetSlotName(slot);
            var json = await CloudProvider?.LoadAsync(name);
            var hasJson = !string.IsNullOrEmpty(json);
            if (hasJson) Persistence.GetFileSystem().Serializer.Deserialize(json, ref gameData);
        }

        // The only available Cloud provider for now is from Unity Service.
        // Maybe add Google
        private static ICloudProvider GetavailableCloudProvider()
        {
#if UNITY_CLOUD_SAVE
            return new UnityCloudService();
#else
            return null;
#endif
        }

        public async Awaitable<IList> LoadAllRemotelyAsync(string playerId)
        {
            var remoteData = await CloudProvider?.LoadAllAsync(playerId);
            var gamesData = new T[remoteData.Count()];
            var serializer = Persistence.GetFileSystem().Serializer;

            for (var i = 0; i < gamesData.Length; i++)
            {
                var content = remoteData.ElementAt(i);
                gamesData[i] = CreateInstance<T>();
                serializer.Deserialize(content, ref gamesData[i]);
            }

            return gamesData;
        }
        #endregion
    }
}