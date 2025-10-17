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
        [Tooltip("The Cloud Provider to use. If none is assigned, the Cloud features will be disabled.")]
        public CloudProviderType cloudProvider;

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

        private void OnValidate() => availableSlots = Mathf.Clamp(availableSlots, 1, MAX_SLOTS);

        public void LoadData(T data)
        {
            // Cannot set SerializeField gameData = data

            var serializer = Persistence.GetFileSystem().Serializer;
            var content = serializer.Serialize(data);

            // SOs are designed to persist data between Scenes only in the Editor, not on Builds.
            // In a Build, a SO resets its values when transitioning between Scenes if not referenced
            // by any reference in memory.

            serializer.Deserialize(content, ref gameData);
            LastSlotIndex = gameData.SlotIndex;
        }

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

            var name = GetSlotName(slot);

            await Persistence.SaveAsync(Data, name);
            if (TryGetCloudProvider(out var cloudProvider))
            {
                var stream = Persistence.LoadStream(name);
                await cloudProvider.SaveAsync(name, FileSystem.COMPRESSED_EXTENSION, stream);
            }

            LastSlotIndex = slot;
            OnSaveFinished?.Invoke();
        }

        /// <summary>
        /// Uploads the current Game Data to the Cloud Service using Public Access so it can be download 
        /// using <see cref="DownloadAsync(int, string, string, CloudProviderType)"/> function.
        /// </summary>
        /// <param name="name">The name of the file to save. Don't use special characters.</param>
        /// <param name="cloudType">The Cloud Provider to use.</param>
        /// <returns>An asynchronous operation.</returns>
        public async Awaitable UploadAsync(string name, CloudProviderType cloudType)
        {
            if (!TryGetCloudProvider(out var provider, cloudType)) return;

            var serializer = persistence.GetFileSystem().Serializer;
            var data = serializer.Serialize(Data);

            await provider.UploadAsync(name, data);
        }

        public async Awaitable DownloadAsync(int slot, string name, string playerId, CloudProviderType cloudType)
        {
            if (!TryGetCloudProvider(out var provider, cloudType)) return;

            var slotName = GetSlotName(slot);
            var content = await provider.DownloadAsync(name, playerId);

            await persistence.SaveAsync(slotName, content);
        }

        public async Awaitable<bool> TryLoadAsync(string path) => await Persistence.TryLoadAsync(Data, path);

        public async Awaitable<bool> TryLoadFromLastSlotAsync()
        {
            Data.ResetData();
            return await TryLoadAsync(LastSlotIndex);
        }

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

            if (TryGetCloudProvider(out var provider))
            {
                //TODO add FileSystem.GetSaveName()
                var cloudName = System.IO.Path.ChangeExtension(name, FileSystem.COMPRESSED_EXTENSION);
                await provider.DeleteAsync(cloudName);
            }

            Persistence.Delete(name);
        }

        public async Awaitable DeleteAllAsync()
        {
            Persistence.DeleteAll();
            if (TryGetCloudProvider(out var provider))
                await provider.DeleteAllAsync();
        }

        /// <summary>
        /// Tries to get the Cloud Provider using the local <see cref="cloudProvider"/>.
        /// </summary>
        /// <param name="provider">The fetched cloud provider instance.</param>
        /// <returns>Whether could find the Cloud provider.</returns>
        public bool TryGetCloudProvider(out ICloudProvider provider) => TryGetCloudProvider(out provider, cloudProvider);

        /// <summary>
        /// Tries to get the Cloud Provider using the given params.
        /// </summary>
        /// <param name="provider"><inheritdoc cref="TryGetCloudProvider(out ICloudProvider)" path="/param[@name='provider']"/></param>
        /// <param name="providerType">The Cloud Provider to use.</param>
        /// <returns><inheritdoc cref="TryGetCloudProvider(out ICloudProvider)"/></returns>
        public bool TryGetCloudProvider(out ICloudProvider provider, CloudProviderType providerType)
        {
            provider = CloudProviderFactory.Create(providerType);
            return provider != null && provider.IsAvailable();
        }

        /// <summary>
        /// Raw file is human legible file (the pretty .json)
        /// </summary>
        /// <returns>Whether should load from the Raw File.</returns>
        public static bool ShouldLoadFromRawFile() => Debug.isDebugBuild;
    }
}