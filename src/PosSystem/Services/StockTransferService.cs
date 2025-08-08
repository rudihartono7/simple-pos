using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using PosSystem.Services.Interfaces;

namespace PosSystem.Services
{
    public class StockTransferService : IStockTransferService
    {
        private readonly ApplicationDbContext _context;

        public StockTransferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockTransfer>> GetAllStockTransfersAsync()
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ShippedByUser)
                .Include(st => st.ReceivedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<StockTransfer?> GetStockTransferByIdAsync(int id)
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ShippedByUser)
                .Include(st => st.ReceivedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.ProductVariant)
                .FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<StockTransfer?> GetStockTransferByNumberAsync(string transferNumber)
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ShippedByUser)
                .Include(st => st.ReceivedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.ProductVariant)
                .FirstOrDefaultAsync(st => st.TransferNumber == transferNumber);
        }

        public async Task<IEnumerable<StockTransfer>> GetStockTransfersBySourceWarehouseAsync(int sourceWarehouseId)
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.FromWarehouseId == sourceWarehouseId)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetStockTransfersByDestinationWarehouseAsync(int destinationWarehouseId)
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.ToWarehouseId == destinationWarehouseId)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetStockTransfersByStatusAsync(string status)
        {
            return await _context.StockTransfers
                .Include(st => st.FromWarehouse)
                .Include(st => st.ToWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.Status == status)
                .OrderByDescending(st => st.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransfer>> GetPendingStockTransfersAsync()
        {
            return await GetStockTransfersByStatusAsync("PENDING");
        }

        public async Task<StockTransfer> CreateStockTransferAsync(StockTransfer stockTransfer)
        {
            stockTransfer.TransferNumber = await GenerateTransferNumberAsync();
            stockTransfer.Status = "PENDING";
            stockTransfer.CreatedAt = DateTime.UtcNow;
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            // Calculate totals
            stockTransfer.TotalQuantity = stockTransfer.StockTransferItems?.Sum(sti => sti.QuantityRequested) ?? 0;
            stockTransfer.TotalValue = stockTransfer.StockTransferItems?.Sum(sti => sti.QuantityRequested * sti.UnitCost) ?? 0;

            _context.StockTransfers.Add(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<StockTransfer> UpdateStockTransferAsync(StockTransfer stockTransfer)
        {
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            // Recalculate totals
            stockTransfer.TotalQuantity = stockTransfer.StockTransferItems?.Sum(sti => sti.QuantityRequested) ?? 0;
            stockTransfer.TotalValue = stockTransfer.StockTransferItems?.Sum(sti => sti.QuantityRequested * sti.UnitCost) ?? 0;

            _context.StockTransfers.Update(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<bool> DeleteStockTransferAsync(int id)
        {
            var stockTransfer = await _context.StockTransfers.FindAsync(id);
            if (stockTransfer == null) return false;

            // Only allow deletion if status is PENDING or DRAFT
            if (stockTransfer.Status != "PENDING" && stockTransfer.Status != "DRAFT")
                return false;

            _context.StockTransfers.Remove(stockTransfer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StockTransfer> ApproveStockTransferAsync(int id, int approvedBy)
        {
            var stockTransfer = await GetStockTransferByIdAsync(id);
            if (stockTransfer == null)
                throw new ArgumentException("Stock transfer not found");

            if (stockTransfer.Status != "PENDING")
                throw new InvalidOperationException("Only pending stock transfers can be approved");

            // Validate stock availability
            if (!await ValidateStockAvailabilityAsync(id))
                throw new InvalidOperationException("Insufficient stock available for transfer");

            stockTransfer.Status = "APPROVED";
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            _context.StockTransfers.Update(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<StockTransfer> ShipStockTransferAsync(int id)
        {
            var stockTransfer = await GetStockTransferByIdAsync(id);
            if (stockTransfer == null)
                throw new ArgumentException("Stock transfer not found");

            if (stockTransfer.Status != "APPROVED")
                throw new InvalidOperationException("Only approved stock transfers can be shipped");

            stockTransfer.Status = "SHIPPED";
            stockTransfer.ShippedDate = DateTime.UtcNow;
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            // Update quantities shipped
            foreach (var item in stockTransfer.StockTransferItems)
            {
                item.QuantityShipped = item.QuantityRequested;
            }

            stockTransfer.TotalQuantity = stockTransfer.StockTransferItems.Sum(sti => sti.QuantityShipped);

            // Reserve stock in source warehouse
            foreach (var item in stockTransfer.StockTransferItems)
            {
                var sourceStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(ws => ws.WarehouseId == stockTransfer.FromWarehouseId &&
                                             ws.ProductId == item.ProductId &&
                                             ws.ProductVariantId == item.ProductVariantId);

                if (sourceStock != null)
                {
                    sourceStock.QuantityReserved += item.QuantityShipped;
                    sourceStock.UpdatedAt = DateTime.UtcNow;
                }
            }

            _context.StockTransfers.Update(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<StockTransfer> CompleteStockTransferAsync(int id, int completedBy)
        {
            var stockTransfer = await GetStockTransferByIdAsync(id);
            if (stockTransfer == null)
                throw new ArgumentException("Stock transfer not found");

            if (stockTransfer.Status != "SHIPPED")
                throw new InvalidOperationException("Only shipped stock transfers can be completed");

            stockTransfer.Status = "COMPLETED";
            stockTransfer.ReceivedBy = completedBy;
            stockTransfer.ReceivedDate = DateTime.UtcNow;
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            // Update quantities received
            foreach (var item in stockTransfer.StockTransferItems)
            {
                item.QuantityReceived = item.QuantityShipped; // Assume all shipped quantity is received
            }

            stockTransfer.TotalQuantity = stockTransfer.StockTransferItems.Sum(sti => sti.QuantityReceived);

            // Update warehouse stock levels
            foreach (var item in stockTransfer.StockTransferItems)
            {
                // Reduce stock from source warehouse
                var sourceStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(ws => ws.WarehouseId == stockTransfer.FromWarehouseId &&
                                             ws.ProductId == item.ProductId &&
                                             ws.ProductVariantId == item.ProductVariantId);

                if (sourceStock != null)
                {
                    sourceStock.QuantityOnHand -= item.QuantityReceived;
                    sourceStock.QuantityReserved -= item.QuantityReceived;
                    sourceStock.UpdatedAt = DateTime.UtcNow;
                }

                // Add stock to destination warehouse
                var destinationStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(ws => ws.WarehouseId == stockTransfer.ToWarehouseId &&
                                             ws.ProductId == item.ProductId &&
                                             ws.ProductVariantId == item.ProductVariantId);

                if (destinationStock != null)
                {
                    destinationStock.QuantityOnHand += item.QuantityReceived;
                    destinationStock.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new stock record for destination warehouse
                    var newStock = new WarehouseStock
                    {
                        WarehouseId = stockTransfer.ToWarehouseId,
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        QuantityOnHand = item.QuantityReceived,
                        QuantityReserved = 0,
                        AverageCost = item.UnitCost,
                        LastCost = item.UnitCost,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.WarehouseStocks.Add(newStock);
                }

                // Create stock movement records
                var sourceMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    MovementType = "TRANSFER_OUT",
                    Quantity = -item.QuantityReceived,
                    UnitCost = item.UnitCost,
                    ReferenceType = "STOCK_TRANSFER",
                    ReferenceId = stockTransfer.Id,
                    Notes = $"Transfer to {stockTransfer.ToWarehouse?.WarehouseName}",
                    UserId = stockTransfer.ReceivedBy ?? stockTransfer.CreatedBy,
                    MovementDate = DateTime.UtcNow
                };
                _context.StockMovements.Add(sourceMovement);

                var destMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    MovementType = "TRANSFER_IN",
                    Quantity = item.QuantityReceived,
                    UnitCost = item.UnitCost,
                    ReferenceType = "STOCK_TRANSFER",
                    ReferenceId = stockTransfer.Id,
                    Notes = $"Transfer from {stockTransfer.FromWarehouse?.WarehouseName}",
                    UserId = stockTransfer.ReceivedBy ?? stockTransfer.CreatedBy,
                    MovementDate = DateTime.UtcNow
                };
                _context.StockMovements.Add(destMovement);
            }

            _context.StockTransfers.Update(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<StockTransfer> CancelStockTransferAsync(int id)
        {
            var stockTransfer = await GetStockTransferByIdAsync(id);
            if (stockTransfer == null)
                throw new ArgumentException("Stock transfer not found");

            if (stockTransfer.Status == "COMPLETED")
                throw new InvalidOperationException("Completed stock transfers cannot be cancelled");

            // If shipped, unreserve stock
            if (stockTransfer.Status == "SHIPPED")
            {
                foreach (var item in stockTransfer.StockTransferItems)
                {
                    var sourceStock = await _context.WarehouseStocks
                        .FirstOrDefaultAsync(ws => ws.WarehouseId == stockTransfer.FromWarehouseId &&
                                                 ws.ProductId == item.ProductId &&
                                                 ws.ProductVariantId == item.ProductVariantId);

                    if (sourceStock != null)
                    {
                        sourceStock.QuantityReserved -= item.QuantityShipped;
                        sourceStock.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            stockTransfer.Status = "CANCELLED";
            stockTransfer.UpdatedAt = DateTime.UtcNow;

            _context.StockTransfers.Update(stockTransfer);
            await _context.SaveChangesAsync();
            return stockTransfer;
        }

        public async Task<string> GenerateTransferNumberAsync()
        {
            var today = DateTime.Today;
            var prefix = $"ST{today:yyyyMMdd}";
            
            var lastTransfer = await _context.StockTransfers
                .Where(st => st.TransferNumber.StartsWith(prefix))
                .OrderByDescending(st => st.TransferNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastTransfer != null)
            {
                var lastNumberStr = lastTransfer.TransferNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }

        public async Task<decimal> CalculateTransferTotalAsync(int stockTransferId)
        {
            var items = await _context.StockTransferItems
                .Where(sti => sti.StockTransferId == stockTransferId)
                .ToListAsync();

            return items.Sum(sti => sti.QuantityRequested * sti.UnitCost);
        }

        public async Task<bool> ValidateStockAvailabilityAsync(int stockTransferId)
        {
            var stockTransfer = await GetStockTransferByIdAsync(stockTransferId);
            if (stockTransfer == null) return false;

            foreach (var item in stockTransfer.StockTransferItems)
            {
                var warehouseStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(ws => ws.WarehouseId == stockTransfer.FromWarehouseId &&
                                             ws.ProductId == item.ProductId &&
                                             ws.ProductVariantId == item.ProductVariantId);

                if (warehouseStock == null || warehouseStock.QuantityAvailable < item.QuantityRequested)
                {
                    return false;
                }
            }

            return true;
        }
    }
}