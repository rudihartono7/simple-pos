using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using PosSystem.Constants;

namespace PosSystem.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly ApplicationDbContext _context;

        public StockMovementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StockMovement> RecordMovementAsync(StockMovement movement)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                movement.MovementDate = DateTime.UtcNow;
                
                // Get the product to update its stock
                var product = await _context.Products.FindAsync(movement.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {movement.ProductId} not found");
                }
                
                // Apply stock movement to product
                var upperMovementType = movement.MovementType.ToUpper();
                
                // Stock increase operations
                 if (upperMovementType == MovementTypes.STOCK_IN ||
                     upperMovementType == MovementTypes.STOCK_IN_LEGACY.ToUpper() ||
                     upperMovementType == MovementTypes.RETURN.ToUpper() ||
                     upperMovementType == MovementTypes.ADJUSTMENT.ToUpper() ||
                     upperMovementType == MovementTypes.ADJUSTMENT_IN ||
                     upperMovementType == MovementTypes.TRANSFER_IN ||
                     upperMovementType == MovementTypes.IN.ToUpper())
                 {
                     product.StockQuantity += (int)movement.Quantity;
                 }
                // Stock decrease operations
                 else if (upperMovementType == MovementTypes.STOCK_OUT ||
                          upperMovementType == MovementTypes.STOCK_OUT_LEGACY.ToUpper() ||
                          upperMovementType == MovementTypes.SALE.ToUpper() ||
                          upperMovementType == MovementTypes.ADJUSTMENT_OUT ||
                          upperMovementType == MovementTypes.TRANSFER_OUT ||
                          upperMovementType == MovementTypes.OUT.ToUpper())
                 {
                     // Ensure we don't go below zero
                     if (product.StockQuantity < (int)movement.Quantity)
                     {
                         throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}, Required: {movement.Quantity}");
                     }
                     product.StockQuantity -= (int)movement.Quantity;
                 }
                // For TRANSFER type, the quantity can be positive or negative
                 else if (upperMovementType == MovementTypes.TRANSFER.ToUpper())
                 {
                     product.StockQuantity += (int)movement.Quantity; // Quantity can be negative for outbound transfers
                     if (product.StockQuantity < 0)
                     {
                         throw new InvalidOperationException($"Transfer would result in negative stock. Available: {product.StockQuantity - (int)movement.Quantity}, Transfer: {movement.Quantity}");
                     }
                 }
                
                // Update product timestamp
                product.UpdatedAt = DateTime.UtcNow;
                
                // Add the stock movement record
                _context.StockMovements.Add(movement);
                
                // Save all changes
                await _context.SaveChangesAsync();
                
                // Commit the transaction
                await transaction.CommitAsync();
                
                return movement;
            }
            catch
            {
                // Rollback the transaction on any error
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByProductAsync(int productId)
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.ProductVariant)
                .Include(sm => sm.User)
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.ProductVariant)
                .Include(sm => sm.User)
                .Where(sm => sm.MovementDate >= startDate && sm.MovementDate <= endDate)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetMovementsByTypeAsync(string movementType)
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.ProductVariant)
                .Include(sm => sm.User)
                .Where(sm => sm.MovementType == movementType)
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();
        }
    }
}