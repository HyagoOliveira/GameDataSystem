using UnityEngine;
using System.Collections.Generic;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Interface for a cloud save provider.
    /// </summary>
    public interface ICloudProvider
    {
        /// <summary>
        /// Whether the cloud provider is available.
        /// </summary>
        /// <returns>True if is play mode and has internet.</returns>
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
        /// <param name="name">
        /// The name of the file to delete. 
        /// Don't forget include the extension.
        /// </param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable DeleteAsync(string name);

        /// <summary>
        /// Saves a file to the cloud provider.
        /// </summary>
        /// <param name="name">
        /// The name of the file to save. 
        /// Don't forget include the extension.
        /// </param>
        /// <param name="file">The byte array containing the file data.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable SaveAsync(string name, byte[] file);

        /// <summary>
        /// Saves a file to the cloud provider using Public Access.
        /// </summary>
        /// <param name="name">
        /// The name of the file to save. 
        /// Don't include the extension.
        /// </param>
        /// <param name="data">The string containing the file data.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable SavePublicAsync(string name, string data);

        /// <summary>
        /// Loads a file from the cloud provider.
        /// </summary>
        /// <param name="name">
        /// The name of the file to load. 
        /// Don't forget include the extension.
        /// </param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<string> LoadAsync(string name);

        /// <summary>
        /// Loads a file from the cloud provider using the given player id. 
        /// Only files saved using Public Access can be loaded.
        /// </summary>
        /// <param name="name">
        /// The name of the file to load. 
        /// Don't forget include the extension.
        /// </param>
        /// <param name="playerId">Player ID to read from.</param>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<string> LoadAsync(string name, string playerId);

        /// <summary>
        /// Loads all file names from the cloud provider.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        Awaitable<List<string>> LoadAllAsync();
    }
}