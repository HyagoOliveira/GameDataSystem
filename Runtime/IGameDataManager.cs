using UnityEngine;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Interface used on objects able to be a Game Data Manager.
    /// </summary>
    /// <remarks>
    /// Process Saving, Loading and Deleting Game Data, both locally and cloud remotely.
    /// </remarks>
    public interface IGameDataManager
    {
        /// <summary>
        /// The total number of available slots.
        /// </summary>
        int AvailableSlots { get; }

        /// <summary>
        /// The Persistence Settings used to save and load data locally.
        /// </summary>
        PersistenceSettings Persistence { get; }

        /// <summary>
        /// Gets the name of the slot based on the given index.
        /// </summary>
        /// <param name="slot">The slot number, starting at 0.</param>
        /// <returns>The name of the slot.</returns>
        string GetSlotName(int slot);

        /// <summary>
        /// Gets the serialized file extension.
        /// </summary>
        /// <returns>The serialized file extension.</returns>
        string GetSerializedExtension();

        /// <summary>
        /// Saves the current Game Data to the last used slot.
        /// </summary>
        /// <returns>An asynchronous saving operation.</returns>
        Awaitable SaveAsync();

        /// <summary>
        /// Saves the current Game Data to the given slot.
        /// </summary>
        /// <param name="slot"><inheritdoc cref="GetSlotName(int)" path="/param[@name='slot']"/></param>
        /// <returns>An asynchronous saving operation.</returns>
        Awaitable SaveAsync(int slot);

        /// <summary>
        /// Returns whether there is a saved Game Data in the last used slot.
        /// </summary>
        /// <returns>An asynchronous loading operation.</returns>
        Awaitable<bool> IsContinueAvailable();

        /// <summary>
        /// Tries to load the Game Data from the given slot.
        /// </summary>
        /// <param name="slot"><inheritdoc cref="GetSlotName(int)" path="/param[@name='slot']"/></param>
        /// <returns>An asynchronous loading operation.</returns>
        Awaitable<bool> TryLoadAsync(int slot);

        /// <summary>
        /// Tries to load the Game Data from the given path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>An asynchronous loading operation.</returns>
        Awaitable<bool> TryLoadAsync(string path);

        /// <summary>
        /// Tries to load the Game Data from the last used slot.
        /// </summary>
        /// <returns>An asynchronous loading operation.</returns>
        Awaitable<bool> TryLoadFromLastSlotAsync();

        /// <summary>
        /// Deletes all saved Game Data both locally and remotely.
        /// </summary>
        /// <returns>An asynchronous deleting operation.</returns>
        Awaitable DeleteAllAsync();

        /// <summary>
        /// Deletes the saved Game Data from the given slot both locally and remotely.
        /// </summary>
        /// <param name="slot"><inheritdoc cref="GetSlotName(int)" path="/param[@name='slot']"/></param>
        /// <returns>An asynchronous deleting operation.</returns>
        Awaitable DeleteAsync(int slot);

        /// <summary>
        /// Uploads the current Game Data to the Cloud Service using Public Access so it can be 
        /// downloaded later by any user.
        /// </summary>
        /// <param name="name">The name of the file to save. Don't use special characters.</param>
        /// <param name="cloudType">The Cloud Provider to use.</param>
        /// <returns>An asynchronous uploading operation.</returns>
        Awaitable UploadAsync(string name, CloudProviderType cloudType);

        /// <summary>
        /// Downloads the Game Data from the Cloud Service.
        /// </summary>
        /// <param name="slot"><inheritdoc cref="GetSlotName(int)" path="/param[@name='slot']"/></param>
        /// <param name="name"><inheritdoc cref="UploadAsync(string, CloudProviderType)" path="/param[@name='name']"/></param>
        /// <param name="playerId">The player identified used to upload the data.</param>
        /// <param name="cloudType"><inheritdoc cref="UploadAsync(string, CloudProviderType)" path="/param[@name='cloudType']"/></param>
        /// <returns>An asynchronous downloading operation.</returns>
        Awaitable DownloadAsync(int slot, string name, string playerId, CloudProviderType cloudType);
    }
}