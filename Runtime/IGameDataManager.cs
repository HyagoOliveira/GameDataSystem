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

        string GetSlotName(int index);
        string GetSerializationExtension();

        void LoadData(object data);
        Awaitable SaveDataAsync();
        Awaitable SaveDataAsync(int slot);

        Awaitable LoadFromLastSlotAsync();
        Awaitable LoadLocallyAsync(int slot);
        Awaitable LoadRemotelyAsync(int slot);

        Awaitable DeleteAsync(int slot);
        Awaitable DeleteAllAsync();

        Awaitable<bool> IsContinueAvailable();

        Awaitable<IList> LoadAllRemotelyAsync(string playerId);
    }
}