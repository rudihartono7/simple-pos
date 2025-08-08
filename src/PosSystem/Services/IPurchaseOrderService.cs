using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string poNumber);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status);
        Task<IEnumerable<PurchaseOrder>> GetPendingPurchaseOrdersAsync();
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder?> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeletePurchaseOrderAsync(int id);
        Task<bool> ApprovePurchaseOrderAsync(int id, int approvedBy);
        Task<bool> CancelPurchaseOrderAsync(int id);
        Task<string> GeneratePurchaseOrderNumberAsync();
        Task<decimal> CalculatePurchaseOrderTotalAsync(int purchaseOrderId);
    }
}