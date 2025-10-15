using UnityEngine;
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

        string GetSlotName(int slot);
        string GetSerializedExtension();

        Awaitable SaveAsync();
        Awaitable SaveAsync(int slot);

        Awaitable<bool> IsContinueAvailable();

        Awaitable<bool> TryLoadAsync(int slot);
        Awaitable<bool> TryLoadAsync(string path);
        Awaitable<bool> TryLoadFromLastSlotAsync();

        Awaitable DeleteAllAsync();
        Awaitable DeleteAsync(int slot);
    }
}