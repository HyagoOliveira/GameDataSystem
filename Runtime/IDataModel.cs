using System.Collections;
using System.Threading.Tasks;

namespace ActionCode.GameDataSystem
{
    public interface IDataModel
    {
        bool HasCloudProvider();

        void SaveData(int slot);
        void UpdateData(object data);

        Task LoadFromLastSlotAsync();
        Task LoadLocallyAsync(int slot);
        Task LoadRemotelyAsync(int slot);

        Task<bool> IsContinueAvailable();
        Task<bool> TryDeleteAsync(int slot);
        Task<bool> TryDeleteAllAsync();

        Task<IList> ListDataAsync();
        Task<IList> LoadAllRemotelyAsync(string playerId);
    }
}
