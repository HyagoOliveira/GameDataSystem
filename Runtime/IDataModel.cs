using UnityEngine;
using System.Collections;

namespace ActionCode.GameDataSystem
{
    public interface IDataModel
    {
        bool HasCloudProvider();

        void SaveData(int slot);
        void UpdateData(object data);

        Awaitable LoadFromLastSlotAsync();
        Awaitable LoadLocallyAsync(int slot);
        Awaitable LoadRemotelyAsync(int slot);

        Awaitable<bool> IsContinueAvailable();
        Awaitable<bool> TryDeleteAsync(int slot);
        Awaitable<bool> TryDeleteAllAsync();

        Awaitable<IList> LoadAllRemotelyAsync(string playerId);
    }
}