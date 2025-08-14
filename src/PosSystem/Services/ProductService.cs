using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using PosSystem.Constants;

namespace PosSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockMovementService _stockMovementService;

        public ProductService(ApplicationDbContext context, IStockMovementService stockMovementService)
        {
            _context = context;
            _stockMovementService = stockMovementService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity, string movementType, int userId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            var oldStock = product.StockQuantity;
            
            var upperMovementType = movementType.ToUpper();
            
            // Stock increase operations
            if (upperMovementType == MovementTypes.STOCK_IN_LEGACY.ToUpper() ||
                upperMovementType == MovementTypes.RETURN.ToUpper() ||
                upperMovementType == MovementTypes.ADJUSTMENT.ToUpper() ||
                upperMovementType == MovementTypes.IN.ToUpper())
            {
                product.StockQuantity += quantity;
            }
            // Stock decrease operations
            else if (upperMovementType == MovementTypes.STOCK_OUT_LEGACY.ToUpper() ||
                     upperMovementType == MovementTypes.SALE.ToUpper() ||
                     upperMovementType == MovementTypes.OUT.ToUpper())
            {
                if (product.StockQuantity < quantity) return false;
                product.StockQuantity -= quantity;
            }
            else
            {
                return false;
            }

            product.UpdatedAt = DateTime.UtcNow;
            
            // Record stock movement
            await _stockMovementService.RecordMovementAsync(new StockMovement
            {
                ProductId = productId,
                MovementType = movementType,
                Quantity = quantity,
                UserId = userId,
                Notes = $"Stock updated from {oldStock} to {product.StockQuantity}"
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && 
                    (p.ProductName.Contains(searchTerm) || 
                     p.ProductCode.Contains(searchTerm) ||
                     (p.Barcode != null && p.Barcode.Contains(searchTerm))))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.StockQuantity <= p.MinStockLevel)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }
    }
}