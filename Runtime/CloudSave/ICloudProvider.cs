using System.Threading.Tasks;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    public interface ICloudProvider
    {
        bool IsUnavailable();
        Task SaveAsync(ScriptableObject data, string name);
        Task<string> LoadAsync(string name, string playerId = null);
        Task<string[]> LoadAllAsync(string playerId);
        Task<string[]> ListRemoteKeys(string playerId = null);
        Task<bool> DeleteAsync(string name);
        Task<bool> DeleteAllAsync();
    }
}