using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using PosSystem.Constants;

namespace PosSystem.Services
{
    public class StockReceivingService : IStockReceivingService
    {
        private readonly ApplicationDbContext _context;

        public StockReceivingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockReceiving>> GetAllStockReceivingsAsync()
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.Supplier)
                .Include(sr => sr.Store)
                .Include(sr => sr.ReceivedByUser)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.Product)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<StockReceiving?> GetStockReceivingByIdAsync(int id)
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.Supplier)
                .Include(sr => sr.Store)
                .Include(sr => sr.ReceivedByUser)
                .Include(sr => sr.PostedByUser)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.Product)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.ProductVariant)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.PurchaseOrderItem)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<StockReceiving?> GetStockReceivingByNumberAsync(string receivingNumber)
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.Supplier)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.Product)
                .FirstOrDefaultAsync(sr => sr.ReceivingNumber == receivingNumber);
        }

        public async Task<IEnumerable<StockReceiving>> GetStockReceivingsByPurchaseOrderAsync(int purchaseOrderId)
        {
            return await _context.StockReceivings
                .Include(sr => sr.Supplier)
                .Include(sr => sr.StockReceivingItems)
                .Where(sr => sr.PurchaseOrderId == purchaseOrderId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockReceiving>> GetStockReceivingsBySupplierAsync(int supplierId)
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.StockReceivingItems)
                .Where(sr => sr.SupplierId == supplierId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockReceiving>> GetStockReceivingsByStatusAsync(string status)
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.Supplier)
                .Include(sr => sr.StockReceivingItems)
                .Where(sr => sr.Status == status)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockReceiving>> GetPendingStockReceivingsAsync()
        {
            return await _context.StockReceivings
                .Include(sr => sr.PurchaseOrder)
                .Include(sr => sr.Supplier)
                .Include(sr => sr.StockReceivingItems)
                    .ThenInclude(sri => sri.Product)
                .Where(sr => sr.Status == "PENDING" || sr.Status == "PARTIAL")
                .OrderBy(sr => sr.ReceivingDate)
                .ToListAsync();
        }

        public async Task<StockReceiving> CreateStockReceivingAsync(StockReceiving stockReceiving)
        {
            stockReceiving.ReceivingNumber = await GenerateReceivingNumberAsync();
            stockReceiving.CreatedAt = DateTime.UtcNow;
            stockReceiving.UpdatedAt = DateTime.UtcNow;

            // Calculate totals
            stockReceiving.TotalQuantity = stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived);
            stockReceiving.TotalCost = stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived * item.UnitCost);

            _context.StockReceivings.Add(stockReceiving);
            await _context.SaveChangesAsync();
            return stockReceiving;
        }

        public async Task<StockReceiving?> UpdateStockReceivingAsync(StockReceiving stockReceiving)
        {
            var existingSR = await _context.StockReceivings
                .Include(sr => sr.StockReceivingItems)
                .FirstOrDefaultAsync(sr => sr.Id == stockReceiving.Id);
            
            if (existingSR == null) return null;

            // Only allow updates if status is PENDING or PARTIAL
            if (existingSR.Status != "PENDING" && existingSR.Status != "PARTIAL")
                return null;

            existingSR.Notes = stockReceiving.Notes;
            existingSR.UpdatedAt = DateTime.UtcNow;

            // Update items
            _context.StockReceivingItems.RemoveRange(existingSR.StockReceivingItems);
            existingSR.StockReceivingItems = stockReceiving.StockReceivingItems;

            // Recalculate totals
            existingSR.TotalQuantity = existingSR.StockReceivingItems.Sum(item => item.QuantityReceived);
            existingSR.TotalCost = existingSR.StockReceivingItems.Sum(item => item.QuantityReceived * item.UnitCost);

            await _context.SaveChangesAsync();
            return existingSR;
        }

        public async Task<bool> DeleteStockReceivingAsync(int id)
        {
            var stockReceiving = await _context.StockReceivings.FindAsync(id);
            if (stockReceiving == null) return false;

            // Only allow deletion if status is PENDING
            if (stockReceiving.Status != "PENDING") return false;

            _context.StockReceivings.Remove(stockReceiving);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteStockReceivingAsync(int id, int completedBy)
        {
            var stockReceiving = await _context.StockReceivings
                .Include(sr => sr.StockReceivingItems)
                .FirstOrDefaultAsync(sr => sr.Id == id);
            
            if (stockReceiving == null) return false;

            if (stockReceiving.Status != "PENDING" && stockReceiving.Status != "PARTIAL")
                return false;

            stockReceiving.Status = "COMPLETED";
            stockReceiving.PostedBy = completedBy;
            stockReceiving.PostedAt = DateTime.UtcNow;
            stockReceiving.UpdatedAt = DateTime.UtcNow;

            // Update stock quantities and create stock movements
            foreach (var item in stockReceiving.StockReceivingItems)
            {
                if (item.QuantityAccepted > 0)
                {
                    // Update product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += (int)item.QuantityAccepted;
                        product.UpdatedAt = DateTime.UtcNow;
                    }

                    // Create stock movement record
                    var stockMovement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        MovementType = MovementTypes.STOCK_IN,
                        Quantity = item.QuantityAccepted,
                        UnitCost = item.UnitCost,
                        ReferenceId = stockReceiving.Id,
                        ReferenceType = "STOCK_RECEIVING",
                        Notes = $"Stock received from {stockReceiving.Supplier?.SupplierName}",
                        BatchNumber = item.BatchNumber,
                        ExpiryDate = item.ExpiryDate,
                        UserId = completedBy,
                        MovementDate = DateTime.UtcNow
                    };

                    _context.StockMovements.Add(stockMovement);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelStockReceivingAsync(int id)
        {
            var stockReceiving = await _context.StockReceivings.FindAsync(id);
            if (stockReceiving == null) return false;

            // Can only cancel if not yet completed
            if (stockReceiving.Status == "COMPLETED") return false;

            stockReceiving.Status = "CANCELLED";
            stockReceiving.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateReceivingNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"RCV{today:yyyyMM}";
            
            var lastReceiving = await _context.StockReceivings
                .Where(sr => sr.ReceivingNumber.StartsWith(prefix))
                .OrderByDescending(sr => sr.ReceivingNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastReceiving != null)
            {
                var lastNumberStr = lastReceiving.ReceivingNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }

        public async Task<bool> ProcessStockReceivingAsync(int id, List<StockReceivingItem> receivedItems)
        {
            var stockReceiving = await _context.StockReceivings
                .Include(sr => sr.StockReceivingItems)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (stockReceiving == null) return false;

            // Update received quantities
            foreach (var receivedItem in receivedItems)
            {
                var existingItem = stockReceiving.StockReceivingItems
                    .FirstOrDefault(sri => sri.Id == receivedItem.Id);
                
                if (existingItem != null)
                {
                    existingItem.QuantityReceived = receivedItem.QuantityReceived;
                    existingItem.QuantityAccepted = receivedItem.QuantityAccepted;
                    existingItem.QuantityRejected = receivedItem.QuantityRejected;
                    existingItem.QualityStatus = receivedItem.QualityStatus;
                    existingItem.Notes = receivedItem.Notes;
                    existingItem.BatchNumber = receivedItem.BatchNumber;
                    existingItem.ExpiryDate = receivedItem.ExpiryDate;
                }
            }

            // Update totals
            stockReceiving.TotalQuantity = stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived);
            stockReceiving.TotalCost = stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived * item.UnitCost);

            // Determine status
            var totalOrdered = stockReceiving.StockReceivingItems.Sum(item => item.QuantityOrdered);
            var totalReceived = stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived);

            if (totalReceived == 0)
                stockReceiving.Status = "PENDING";
            else if (totalReceived < totalOrdered)
                stockReceiving.Status = "PARTIAL";
            else
                stockReceiving.Status = "RECEIVED";

            stockReceiving.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateReceivingTotalAsync(int stockReceivingId)
        {
            var stockReceiving = await _context.StockReceivings
                .Include(sr => sr.StockReceivingItems)
                .FirstOrDefaultAsync(sr => sr.Id == stockReceivingId);

            if (stockReceiving == null) return 0;

            return stockReceiving.StockReceivingItems.Sum(item => item.QuantityReceived * item.UnitCost);
        }
    }
}