using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IStoreService
    {
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<Store?> GetStoreByIdAsync(int id);
        Task<Store?> GetStoreByCodeAsync(string storeCode);
        Task<Store> CreateStoreAsync(Store store);
        Task<Store> UpdateStoreAsync(Store store);
        Task DeleteStoreAsync(int id);
        Task<string> GenerateStoreCodeAsync();
        Task<bool> StoreCodeExistsAsync(string storeCode);
    }
}