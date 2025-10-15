#if UNITY_CLOUD_SAVE && UNITY_AUTHENTICATION
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Authentication;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Provider for Unity Cloud Save service.
    /// </summary>
    public sealed class UnityCloudProvider : ICloudProvider
    {
        public static IPlayerDataService PlayerData => CloudSaveService.Instance.Data.Player;
        public static IPlayerFilesService PlayerFiles => CloudSaveService.Instance.Files.Player;

        public async Awaitable DeleteAllAsync()
        {
            await CheckSignInAsync();

            var files = await PlayerFiles.ListAllAsync();
            foreach (var file in files)
            {
                var name = file.Key;
                await TryDelete(name);
            }
        }

        public async Awaitable DeleteAsync(string name)
        {
            await CheckSignInAsync();
            await TryDelete(name);
        }

        public async Awaitable SaveAsync(string name, byte[] file)
        {
            await CheckSignInAsync();
            await PlayerFiles.SaveAsync(name, file);
        }

        public async Awaitable SavePubliclyAsync(string name, string data)
        {
            await CheckSignInAsync();

            name = System.IO.Path.GetFileNameWithoutExtension(name);
            var remoteData = new Dictionary<string, object> { { name, data } };
            var options = new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions());
            await PlayerData.SaveAsync(remoteData, options);
        }

        public async Awaitable<string> LoadAsync(string name)
        {
            await CheckSignInAsync();
            var data = await PlayerFiles.LoadBytesAsync(name);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public async Awaitable<string> LoadAsync(string name, string playerId)
        {
            await CheckSignInAsync();

            var options = new LoadOptions(new PublicReadAccessClassOptions(playerId));
            var data = await PlayerData.LoadAsync(new HashSet<string> { name }, options);
            var wasLoaded = data.TryGetValue(name, out var remoteData);

            return wasLoaded ? remoteData.Value.GetAsString() : string.Empty;
        }

        public async Awaitable<List<string>> LoadAllAsync()
        {
            await CheckSignInAsync();

            var list = new List<string>();
            var files = await PlayerFiles.ListAllAsync();

            foreach (var file in files)
            {
                var name = file.Key;
                var bytes = await PlayerFiles.LoadBytesAsync(name);
                var data = System.Text.Encoding.UTF8.GetString(bytes);
                list.Add(data);
            }

            return list;
        }

        private static async Awaitable CheckSignInAsync()
        {
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn) return;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async Awaitable TryDelete(string name)
        {
            try
            {
                await PlayerFiles.DeleteAsync(name);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
#endif