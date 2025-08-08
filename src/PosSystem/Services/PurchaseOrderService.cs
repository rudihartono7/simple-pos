using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Store)
                .Include(po => po.CreatedByUser)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Product)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Store)
                .Include(po => po.CreatedByUser)
                .Include(po => po.ApprovedByUser)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Product)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.ProductVariant)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByNumberAsync(string poNumber)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Product)
                .FirstOrDefaultAsync(po => po.PurchaseOrderNumber == poNumber);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                .Where(po => po.SupplierId == supplierId)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                .Where(po => po.Status == status)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPendingPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Product)
                .Where(po => po.Status == "PENDING" || po.Status == "APPROVED" || po.Status == "ORDERED")
                .OrderBy(po => po.ExpectedDeliveryDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.PurchaseOrderNumber = await GeneratePurchaseOrderNumberAsync();
            purchaseOrder.CreatedAt = DateTime.UtcNow;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            // Calculate totals
            purchaseOrder.SubTotal = purchaseOrder.PurchaseOrderItems.Sum(item => item.TotalCost);
            purchaseOrder.TotalAmount = purchaseOrder.SubTotal + purchaseOrder.TaxAmount + purchaseOrder.ShippingCost - purchaseOrder.DiscountAmount;

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();
            return purchaseOrder;
        }

        public async Task<PurchaseOrder?> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var existingPO = await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderItems)
                .FirstOrDefaultAsync(po => po.Id == purchaseOrder.Id);
            
            if (existingPO == null) return null;

            // Only allow updates if status is DRAFT or PENDING
            if (existingPO.Status != "DRAFT" && existingPO.Status != "PENDING")
                return null;

            existingPO.SupplierId = purchaseOrder.SupplierId;
            existingPO.StoreId = purchaseOrder.StoreId;
            existingPO.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
            existingPO.Notes = purchaseOrder.Notes;
            existingPO.Terms = purchaseOrder.Terms;
            existingPO.TaxAmount = purchaseOrder.TaxAmount;
            existingPO.DiscountAmount = purchaseOrder.DiscountAmount;
            existingPO.ShippingCost = purchaseOrder.ShippingCost;
            existingPO.UpdatedAt = DateTime.UtcNow;

            // Update items
            _context.PurchaseOrderItems.RemoveRange(existingPO.PurchaseOrderItems);
            existingPO.PurchaseOrderItems = purchaseOrder.PurchaseOrderItems;

            // Recalculate totals
            existingPO.SubTotal = existingPO.PurchaseOrderItems.Sum(item => item.TotalCost);
            existingPO.TotalAmount = existingPO.SubTotal + existingPO.TaxAmount + existingPO.ShippingCost - existingPO.DiscountAmount;

            await _context.SaveChangesAsync();
            return existingPO;
        }

        public async Task<bool> DeletePurchaseOrderAsync(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null) return false;

            // Only allow deletion if status is DRAFT
            if (purchaseOrder.Status != "DRAFT") return false;

            _context.PurchaseOrders.Remove(purchaseOrder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApprovePurchaseOrderAsync(int id, int approvedBy)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null) return false;

            if (purchaseOrder.Status != "PENDING") return false;

            purchaseOrder.Status = "APPROVED";
            purchaseOrder.ApprovedBy = approvedBy;
            purchaseOrder.ApprovedAt = DateTime.UtcNow;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPurchaseOrderAsync(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null) return false;

            // Can only cancel if not yet received
            if (purchaseOrder.Status == "RECEIVED" || purchaseOrder.Status == "PARTIAL_RECEIVED")
                return false;

            purchaseOrder.Status = "CANCELLED";
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GeneratePurchaseOrderNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"PO{today:yyyyMM}";
            
            var lastPO = await _context.PurchaseOrders
                .Where(po => po.PurchaseOrderNumber.StartsWith(prefix))
                .OrderByDescending(po => po.PurchaseOrderNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastPO != null)
            {
                var lastNumberStr = lastPO.PurchaseOrderNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }

        public async Task<decimal> CalculatePurchaseOrderTotalAsync(int purchaseOrderId)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderItems)
                .FirstOrDefaultAsync(po => po.Id == purchaseOrderId);

            if (purchaseOrder == null) return 0;

            var subTotal = purchaseOrder.PurchaseOrderItems.Sum(item => item.TotalCost);
            return subTotal + purchaseOrder.TaxAmount + purchaseOrder.ShippingCost - purchaseOrder.DiscountAmount;
        }
    }
}