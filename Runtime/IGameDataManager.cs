using UnityEngine;
using System.Collections;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Interface used on objects able to be a Game Data Manager.
    /// <para>
    /// Save, load and delete game data both locally and cloud remotely.
    /// </para>
    /// </summary>
    public interface IGameDataManager
    {
        /// <summary>
        /// The total number of available slots.
        /// </summary>
        int AvailableSlots { get; }
        PersistenceSettings Persistence { get; }

        bool HasCloudProvider();

        string GetSlotName(int slot);
        string GetSerializedExtension();

        void LoadData(object data);
        Awaitable SaveAsync();
        Awaitable SaveAsync(int slot);

        Awaitable<bool> TryLoadFromLastSlotAsync();
        Awaitable<bool> TryLoadAsync(int slot);
        Awaitable<bool> TryLoadAsync(string path);
        Awaitable LoadRemotelyAsync(int slot);

        Awaitable DeleteAsync(int slot);
        Awaitable DeleteAllAsync();

        Awaitable<bool> IsContinueAvailable();

        Awaitable<IList> LoadAllRemotelyAsync(string playerId);
    }
}