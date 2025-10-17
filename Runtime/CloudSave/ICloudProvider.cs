using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Interface for objects able to be Cloud Provider.
    /// </summary>
    public interface ICloudProvider
    {
        /// <summary>
        /// Returns the current player Cloud ID.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<string> GetPlayerId();

        /// <summary>
        /// Whether the cloud provider is available.
        /// </summary>
        /// <returns>True if is play mode and has Internet.</returns>
        bool IsAvailable() =>
            Application.isPlaying &&
            Application.internetReachability != NetworkReachability.NotReachable;

        /// <summary>
        /// Deletes all files from the cloud provider.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        Awaitable DeleteAllAsync();

        /// <summary>
        /// Deletes a file from the cloud provider.
        /// </summary>
        /// <param name="name">The file name. Don't use any special characters. 
        /// </param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable DeleteAsync(string name);

        /// <summary>
        /// Saves a file to the cloud provider.
        /// </summary>
        /// <param name="name"><inheritdoc cref="DeleteAsync(string)" path="/param[@name='name']"/></param>
        /// <param name="extension">The file extension.</param>
        /// <param name="stream">The Stream containing the file data.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable SaveAsync(string name, string extension, Stream stream);

        /// <summary>
        /// Loads a file from the cloud provider.
        /// </summary>
        /// <param name="name"><inheritdoc cref="DeleteAsync(string)" path="/param[@name='name']"/></param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<string> LoadAsync(string name);

        /// <summary>
        /// Loads all the file names from the cloud provider.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<List<string>> LoadAllNamesAsync();

        /// <summary>
        /// Uploads the given data content using Public Access so it can be download for other players.
        /// </summary>
        /// <param name="name"><inheritdoc cref="DeleteAsync(string)" path="/param[@name='name']"/></param>
        /// <param name="content">The data content.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable UploadAsync(string name, string content);

        /// <summary>
        /// Downloads a file from the cloud provider using the given file name and the player id. 
        /// Only files uploaded using Public Access can be downloaded.
        /// </summary>
        /// <param name="name"><inheritdoc cref="DeleteAsync(string)" path="/param[@name='name']"/></param>
        /// <param name="playerId">Player ID to read from.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<string> DownloadAsync(string name, string playerId);
    }
}