using UnityEngine;

namespace ActionCode.GameDataSystem
{
    public interface ICloudProvider
    {
        bool IsUnavailable();
        Awaitable SaveAsync(ScriptableObject data, string name);
        Awaitable<string> LoadAsync(string name, string playerId = null);
        Awaitable<string[]> LoadAllAsync(string playerId);
        Awaitable<string[]> ListRemoteKeys(string playerId = null);
        Awaitable<bool> DeleteAsync(string name);
        Awaitable<bool> DeleteAllAsync();
    }
}