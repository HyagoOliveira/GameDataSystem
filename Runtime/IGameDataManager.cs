using UnityEngine;
using System.Collections;

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
        bool HasCloudProvider();

        void SaveData(int slot);
        void LoadData(object data);

        Awaitable LoadFromLastSlotAsync();
        Awaitable LoadLocallyAsync(int slot);
        Awaitable LoadRemotelyAsync(int slot);

        Awaitable<bool> IsContinueAvailable();
        Awaitable<bool> TryDeleteAllAsync();
        Awaitable<bool> TryDeleteAsync(int slot);

        Awaitable<IList> LoadAllRemotelyAsync(string playerId);
    }
}