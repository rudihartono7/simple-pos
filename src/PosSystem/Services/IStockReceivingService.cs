using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IStockReceivingService
    {
        Task<IEnumerable<StockReceiving>> GetAllStockReceivingsAsync();
        Task<StockReceiving?> GetStockReceivingByIdAsync(int id);
        Task<StockReceiving?> GetStockReceivingByNumberAsync(string receivingNumber);
        Task<IEnumerable<StockReceiving>> GetStockReceivingsByPurchaseOrderAsync(int purchaseOrderId);
        Task<IEnumerable<StockReceiving>> GetStockReceivingsBySupplierAsync(int supplierId);
        Task<IEnumerable<StockReceiving>> GetStockReceivingsByStatusAsync(string status);
        Task<IEnumerable<StockReceiving>> GetPendingStockReceivingsAsync();
        Task<StockReceiving> CreateStockReceivingAsync(StockReceiving stockReceiving);
        Task<StockReceiving?> UpdateStockReceivingAsync(StockReceiving stockReceiving);
        Task<bool> DeleteStockReceivingAsync(int id);
        Task<bool> CompleteStockReceivingAsync(int id, int completedBy);
        Task<bool> CancelStockReceivingAsync(int id);
        Task<string> GenerateReceivingNumberAsync();
        Task<bool> ProcessStockReceivingAsync(int id, List<StockReceivingItem> receivedItems);
        Task<decimal> CalculateReceivingTotalAsync(int stockReceivingId);
    }
}