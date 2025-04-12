using TNRD;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    public abstract class AbstractGameDataSettings<T> : ScriptableObject where T : AbstractGameData
    {
        [SerializeField] private T gameData;
        [SerializeField] private PersistenceSettings persistenceSettings;
        [SerializeField] private SerializableInterface<ICloudProvider> cloudProvider;

        /// <summary>
        /// The Game main data.
        /// </summary>
        public T Data => gameData;

        /// <summary>
        /// The serialization settings.
        /// </summary>
        public PersistenceSettings Serialization => persistenceSettings;

        public ICloudProvider CloudProvider => cloudProvider.Value;

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

        public async Task LoadFromLastSlotAsync()
        {
            var slot = persistenceSettings.GetLastSlot();
            var wasLoaded = await persistenceSettings.TryLoadLastSlot(Data);
            Debug.Log($"Was data loaded from last slot {slot}? {wasLoaded}");
        }

        public async Task LoadLocallyAsync(int slot)
        {
            var wasLoaded = await persistenceSettings.TryLoad(Data, slot);
            Debug.Log($"Was {Data.name} loaded from slot {slot}? {wasLoaded}");
        }

        public async Task LoadRemotelyAsync(int slot)
        {
            var name = persistenceSettings.GetSlotName(slot);
            var json = await CloudProvider?.LoadAsync(name);
            var hasJson = !string.IsNullOrEmpty(json);
            if (hasJson) persistenceSettings.GetFileSystem().Serializer.Deserialize(json, ref gameData);
        }

        public async Task<bool> IsContinueAvailable() => await HasLastSlotAvailable();

        public async Task<bool> TryDeleteAsync(int slot)
        {
            await CloudProvider?.DeleteAsync(persistenceSettings.GetSlotName(slot));
            return TryDeleteLocally(slot);
        }

        public async Task<bool> TryDeleteAllAsync() =>
            TryDeleteAllLocally() &&
            await CloudProvider?.DeleteAllAsync();

        public async Task<IList> ListSlotsAsync()
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

        public async Task<IList> LoadAllRemotelyAsync(string playerId)
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

        protected virtual async Task SaveLocallyAsync(ScriptableObject data, int slot)
        {
            var wasSaved = await persistenceSettings.Save(data, slot);
            Debug.Log($"Was {data.name} locally saved in slot {slot}? {wasSaved}");
        }

        protected virtual bool TryDeleteLocally(int slot)
        {
            var wasDeleted = persistenceSettings.TryDelete(slot);
            Debug.Log($"Was game data deleted from slot {slot}? {wasDeleted}");
            return wasDeleted;
        }

        protected virtual bool TryDeleteAllLocally()
        {
            var wasDeleted = persistenceSettings.TryDeleteAll();
            Debug.Log($"Was all data deleted? {wasDeleted}");
            return wasDeleted;
        }

        private async Task<bool> HasLastSlotAvailable()
        {
            var data = CreateInstance<T>();
            return await persistenceSettings.TryLoadLastSlot(data);
        }
    }
}