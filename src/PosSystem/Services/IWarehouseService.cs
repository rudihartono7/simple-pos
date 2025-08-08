using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse?> GetWarehouseByIdAsync(int id);
        Task<Warehouse?> GetWarehouseByCodeAsync(string warehouseCode);
        Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync();
        Task<IEnumerable<Warehouse>> GetWarehousesByTypeAsync(string warehouseType);
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        Task<Warehouse?> UpdateWarehouseAsync(Warehouse warehouse);
        Task<bool> DeleteWarehouseAsync(int id);
        Task<bool> WarehouseCodeExistsAsync(string warehouseCode);
        Task<IEnumerable<WarehouseStock>> GetWarehouseStockAsync(int warehouseId);
        Task<WarehouseStock?> GetWarehouseStockByProductAsync(int warehouseId, int productId, int? productVariantId = null);
        Task<bool> UpdateWarehouseStockAsync(int warehouseId, int productId, int? productVariantId, int quantity, string movementType);
        Task<IEnumerable<WarehouseStock>> GetLowStockItemsAsync(int warehouseId);
    }
}