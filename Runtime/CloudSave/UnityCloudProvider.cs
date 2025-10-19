#if UNITY_CLOUD_SAVE && UNITY_AUTHENTICATION
using System.IO;
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

        public override string ToString() => "Unity Cloud Save";

        public async Awaitable<string> GetUserIdAsync()
        {
            await CheckSignInAsync();
            return AuthenticationService.Instance.PlayerId;
        }

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

        public async Awaitable SaveAsync(string name, string extension, Stream stream)
        {
            await CheckSignInAsync();
            await PlayerFiles.SaveAsync($"{name}.{extension}", stream);
        }

        public async Awaitable<string> LoadAsync(string name)
        {
            await CheckSignInAsync();
            var data = await PlayerFiles.LoadBytesAsync(name);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public async Awaitable<List<string>> LoadAllNamesAsync()
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

        public async Awaitable UploadAsync(string name, string content)
        {
            await CheckSignInAsync();
            var remoteData = new Dictionary<string, object> { { GetValidKey(name), content } };
            var options = new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions());
            await PlayerData.SaveAsync(remoteData, options);
        }

        public async Awaitable<string> DownloadAsync(string name, string userId)
        {
            await CheckSignInAsync();

            var options = new LoadOptions(new PublicReadAccessClassOptions(userId));
            var data = await PlayerData.LoadAsync(new HashSet<string> { name }, options);
            var wasLoaded = data.TryGetValue(name, out var remoteData);

            return wasLoaded ? remoteData.Value.GetAsString() : string.Empty;
        }

        public static string GetValidKey(string name) => name.
            Replace(" ", "_").
            Replace(".", "_").
            Trim();

        private async Awaitable TryDelete(string name)
        {
            try
            {
                await PlayerFiles.DeleteAsync(name);
            }
            catch (System.Exception e)
            {
                // HttpException is a internal class, so we check the message instead.
                var wasFileNotFound = e.Message.ToLower() == "file not found";
                if (!wasFileNotFound) Debug.LogException(e);
            }
        }

        private static async Awaitable CheckSignInAsync()
        {
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn) return;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
#endif