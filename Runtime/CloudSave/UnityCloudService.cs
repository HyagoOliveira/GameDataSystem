#if UNITY_CLOUD_SAVE
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using DeleteOptions = Unity.Services.CloudSave.Models.Data.Player.DeleteOptions;
using DeleteAllOptions = Unity.Services.CloudSave.Models.Data.Player.DeleteAllOptions;

namespace ActionCode.GameDataSystem
{
    public sealed class UnityCloudService : ICloudProvider
    {
        public IPlayerDataService CloudPlayer => CloudSaveService.Instance.Data.Player;

        public bool IsUnavailable() => !Application.isPlaying;

        public async Task SaveAsync(ScriptableObject data, string name)
        {
            if (IsUnavailable()) return;

            var remoteData = new Dictionary<string, object> { { name, data } };
            var options = new SaveOptions(new PublicWriteAccessClassOptions());
            var savedData = await CloudSaveService.Instance.Data.Player.SaveAsync(remoteData, options);
            var wasSaved = savedData.Count > 0;

            Debug.Log($"Was {name} saved remotely? {wasSaved}");
        }

        public async Task<string> LoadAsync(string name, string playerId = null)
        {
            if (IsUnavailable()) return string.Empty;

            var options = new LoadOptions(new PublicReadAccessClassOptions(playerId));
            var data = await CloudPlayer.LoadAsync(new HashSet<string> { name }, options);
            var wasLoaded = data.TryGetValue(name, out var remoteData);

            Debug.Log($"Was {name} loaded remotely? {wasLoaded}");

            return wasLoaded ? remoteData.Value.GetAsString() : string.Empty;
        }

        public async Task<string[]> LoadAllAsync(string playerId)
        {
            if (IsUnavailable()) return null;

            var options = new LoadAllOptions(new PublicReadAccessClassOptions(playerId));
            var data = await CloudPlayer.LoadAllAsync(options);
            var wasLoaded = data.Count > 0;

            Debug.Log($"Was all data loaded remotely? {wasLoaded}");

            var i = 0;
            var remoteData = new string[data.Count];
            foreach (var item in data.Values)
            {
                remoteData[i++] = item.Value.GetAsString();
            }

            return remoteData;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            if (IsUnavailable()) return false;

            try
            {
                var options = new DeleteOptions(new PublicWriteAccessClassOptions());
                await CloudPlayer.DeleteAsync(name, options);
                Debug.Log($"{name} was deleted remotely.");
                return true;
            }
            catch (CloudSaveException exception)
            {
                var wasDataNotFound = exception.Reason == CloudSaveExceptionReason.NotFound;
                if (wasDataNotFound)
                {
                    Debug.LogWarning($"{name} was not found in Unity Cloud. Nothing deleted from there.");
                    return true;
                }
            }
            catch (System.Exception exception)
            {
                Debug.Log($"{name} was not deleted remotely.");
                Debug.LogError(exception);
            }
            return false;
        }

        public async Task<bool> DeleteAllAsync()
        {
            if (IsUnavailable()) return false;

            try
            {
                var options = new DeleteAllOptions(new PublicWriteAccessClassOptions());
                await CloudPlayer.DeleteAllAsync(options);
                Debug.Log("All data was deleted remotely.");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log("All data was not deleted remotely.");
                Debug.LogError(e);
            }
            return false;
        }

        public async Task<string[]> ListRemoteKeys(string playerId = null)
        {
            if (IsUnavailable()) return new string[0];

            var options = new ListAllKeysOptions(new PublicReadAccessClassOptions(playerId));
            var asyncKeys = await CloudPlayer.ListAllKeysAsync(options);
            var keys = new string[asyncKeys.Count];

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = asyncKeys[i].Key;
            }

            return keys;
        }
    }
}
#endif