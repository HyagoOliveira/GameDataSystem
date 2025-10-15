using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Interface for a cloud save provider.
    /// </summary>
    public interface ICloudProvider
    {
        bool IsAvailable();
        Awaitable SaveAsync(ScriptableObject data, string name);
        Awaitable<string> LoadAsync(string name, string playerId = null);
        Awaitable<string[]> LoadAllAsync(string playerId);
        Awaitable<string[]> ListRemoteKeys(string playerId = null);
        Awaitable<bool> DeleteAsync(string name);
        Awaitable<bool> DeleteAllAsync();
    }
}