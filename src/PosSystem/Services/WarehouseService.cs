using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();
        }

        public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
        {
            return await _context.Warehouses
                .Include(w => w.WarehouseStocks)
                    .ThenInclude(ws => ws.Product)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Warehouse?> GetWarehouseByCodeAsync(string warehouseCode)
        {
            return await _context.Warehouses
                .FirstOrDefaultAsync(w => w.WarehouseCode == warehouseCode);
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _context.Warehouses
                .Where(w => w.IsActive)
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesByTypeAsync(string warehouseType)
        {
            return await _context.Warehouses
                .Where(w => w.WarehouseType == warehouseType && w.IsActive)
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            warehouse.CreatedAt = DateTime.UtcNow;
            warehouse.UpdatedAt = DateTime.UtcNow;

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task<Warehouse?> UpdateWarehouseAsync(Warehouse warehouse)
        {
            var existingWarehouse = await _context.Warehouses.FindAsync(warehouse.Id);
            if (existingWarehouse == null) return null;

            existingWarehouse.WarehouseName = warehouse.WarehouseName;
            existingWarehouse.WarehouseCode = warehouse.WarehouseCode;
            existingWarehouse.WarehouseType = warehouse.WarehouseType;
            existingWarehouse.Address = warehouse.Address;
            existingWarehouse.City = warehouse.City;
            existingWarehouse.PostalCode = warehouse.PostalCode;
            existingWarehouse.Phone = warehouse.Phone;
            existingWarehouse.Email = warehouse.Email;
            existingWarehouse.ContactPerson = warehouse.ContactPerson;
            existingWarehouse.IsActive = warehouse.IsActive;
            existingWarehouse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingWarehouse;
        }

        public async Task<bool> DeleteWarehouseAsync(int id)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.WarehouseStocks)
                .FirstOrDefaultAsync(w => w.Id == id);
            
            if (warehouse == null) return false;

            // Check if warehouse has stock
            if (warehouse.WarehouseStocks.Any(ws => ws.QuantityOnHand > 0))
                return false; // Cannot delete warehouse with stock

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WarehouseCodeExistsAsync(string warehouseCode)
        {
            return await _context.Warehouses
                .AnyAsync(w => w.WarehouseCode == warehouseCode);
        }

        public async Task<IEnumerable<WarehouseStock>> GetWarehouseStockAsync(int warehouseId)
        {
            return await _context.WarehouseStocks
                .Include(ws => ws.Product)
                .Include(ws => ws.ProductVariant)
                .Where(ws => ws.WarehouseId == warehouseId)
                .OrderBy(ws => ws.Product.ProductName)
                .ToListAsync();
        }

        public async Task<WarehouseStock?> GetWarehouseStockByProductAsync(int warehouseId, int productId, int? productVariantId = null)
        {
            return await _context.WarehouseStocks
                .Include(ws => ws.Product)
                .Include(ws => ws.ProductVariant)
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId 
                    && ws.ProductId == productId 
                    && ws.ProductVariantId == productVariantId);
        }

        public async Task<bool> UpdateWarehouseStockAsync(int warehouseId, int productId, int? productVariantId, int quantity, string movementType)
        {
            var warehouseStock = await GetWarehouseStockByProductAsync(warehouseId, productId, productVariantId);
            
            if (warehouseStock == null)
            {
                // Create new warehouse stock record
                warehouseStock = new WarehouseStock
                {
                    WarehouseId = warehouseId,
                    ProductId = productId,
                    ProductVariantId = productVariantId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    MinStockLevel = 0,
                    MaxStockLevel = 1000,
                    ReorderPoint = 10,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.WarehouseStocks.Add(warehouseStock);
            }

            // Update quantity based on movement type
            switch (movementType.ToUpper())
            {
                case "STOCK_IN":
                case "TRANSFER_IN":
                case "ADJUSTMENT_IN":
                    warehouseStock.QuantityOnHand += quantity;
                    break;
                case "STOCK_OUT":
                case "TRANSFER_OUT":
                case "ADJUSTMENT_OUT":
                case "SALE":
                    warehouseStock.QuantityOnHand -= quantity;
                    break;
                case "RESERVE":
                    warehouseStock.QuantityReserved += quantity;
                    break;
                case "UNRESERVE":
                    warehouseStock.QuantityReserved -= quantity;
                    break;
            }

            // Ensure quantities don't go negative
            if (warehouseStock.QuantityOnHand < 0) warehouseStock.QuantityOnHand = 0;
            if (warehouseStock.QuantityReserved < 0) warehouseStock.QuantityReserved = 0;

            // Update available quantity - this is a calculated property, no need to assign
            warehouseStock.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WarehouseStock>> GetLowStockItemsAsync(int warehouseId)
        {
            return await _context.WarehouseStocks
                .Include(ws => ws.Product)
                .Include(ws => ws.ProductVariant)
                .Where(ws => ws.WarehouseId == warehouseId 
                    && ws.QuantityOnHand <= ws.ReorderPoint)
                .OrderBy(ws => ws.Product.ProductName)
                .ToListAsync();
        }
    }
}