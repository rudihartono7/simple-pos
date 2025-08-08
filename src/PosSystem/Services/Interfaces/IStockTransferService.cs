using PosSystem.Models;

namespace PosSystem.Services.Interfaces
{
    public interface IStockTransferService
    {
        // Basic CRUD operations
        Task<IEnumerable<StockTransfer>> GetAllStockTransfersAsync();
        Task<StockTransfer?> GetStockTransferByIdAsync(int id);
        Task<StockTransfer?> GetStockTransferByNumberAsync(string transferNumber);
        Task<IEnumerable<StockTransfer>> GetStockTransfersBySourceWarehouseAsync(int sourceWarehouseId);
        Task<IEnumerable<StockTransfer>> GetStockTransfersByDestinationWarehouseAsync(int destinationWarehouseId);
        Task<IEnumerable<StockTransfer>> GetStockTransfersByStatusAsync(string status);
        Task<IEnumerable<StockTransfer>> GetPendingStockTransfersAsync();
        
        // Create, Update, Delete operations
        Task<StockTransfer> CreateStockTransferAsync(StockTransfer stockTransfer);
        Task<StockTransfer> UpdateStockTransferAsync(StockTransfer stockTransfer);
        Task<bool> DeleteStockTransferAsync(int id);
        
        // Business operations
        Task<StockTransfer> ApproveStockTransferAsync(int id, int approvedBy);
        Task<StockTransfer> ShipStockTransferAsync(int id);
        Task<StockTransfer> CompleteStockTransferAsync(int id, int completedBy);
        Task<StockTransfer> CancelStockTransferAsync(int id);
        
        // Utility methods
        Task<string> GenerateTransferNumberAsync();
        Task<decimal> CalculateTransferTotalAsync(int stockTransferId);
        Task<bool> ValidateStockAvailabilityAsync(int stockTransferId);
    }
}