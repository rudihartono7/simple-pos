using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;
using System.Security.Claims;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductService productService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
            }
        }

        [HttpGet("by-barcode/{barcode}")]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            try
            {
                var product = await _productService.GetProductByBarcodeAsync(barcode);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var products = await _productService.SearchProductsAsync(searchTerm);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching products", error = ex.Message });
            }
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            try
            {
                var products = await _productService.GetLowStockProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving low stock products", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            try
            {
                var product = new Product
                {
                    ProductName = request.ProductName,
                    Description = request.Description,
                    ProductCode = request.ProductCode,
                    Barcode = request.Barcode,
                    UnitOfMeasure = request.UnitOfMeasure,
                    UnitPrice = request.UnitPrice,
                    CostPrice = request.CostPrice,
                    StockQuantity = request.StockQuantity,
                    MinStockLevel = request.MinStockLevel,
                    CategoryId = request.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (request.Image != null && request.Image.Length > 0)
                {
                    var imageUrl = await SaveProductImageAsync(request.Image);
                    product.ImageUrl = imageUrl;
                }

                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        {
            try
            {
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                existingProduct.ProductName = request.ProductName;
                existingProduct.Description = request.Description;
                existingProduct.Barcode = request.Barcode;
                existingProduct.UnitOfMeasure = request.UnitOfMeasure;
                existingProduct.UnitPrice = request.UnitPrice;
                existingProduct.CostPrice = request.CostPrice;
                existingProduct.StockQuantity = request.StockQuantity;
                existingProduct.MinStockLevel = request.MinStockLevel;
                existingProduct.CategoryId = request.CategoryId;
                existingProduct.IsActive = request.IsActive;

                // Handle image upload
                if (request.Image != null && request.Image.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        await DeleteProductImageAsync(existingProduct.ImageUrl);
                    }
                    
                    var imageUrl = await SaveProductImageAsync(request.Image);
                    existingProduct.ImageUrl = imageUrl;
                }

                var updatedProduct = await _productService.UpdateProductAsync(existingProduct);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Delete product image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await DeleteProductImageAsync(product.ImageUrl);
                }

                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product", error = ex.Message });
            }
        }

        [HttpPost("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var result = await _productService.UpdateStockAsync(id, request.Quantity, request.MovementType, userId);
                if (!result)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(new { message = "Stock updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating stock", error = ex.Message });
            }
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadProductImage(int id, IFormFile image)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                if (image == null || image.Length == 0)
                {
                    return BadRequest(new { message = "No image file provided" });
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await DeleteProductImageAsync(product.ImageUrl);
                }

                var imageUrl = await SaveProductImageAsync(image);
                product.ImageUrl = imageUrl;

                await _productService.UpdateProductAsync(product);

                return Ok(new { message = "Image uploaded successfully", imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the image", error = ex.Message });
            }
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    return BadRequest(new { message = "Product has no image to delete" });
                }

                await DeleteProductImageAsync(product.ImageUrl);
                product.ImageUrl = null;

                await _productService.UpdateProductAsync(product);

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the image", error = ex.Message });
            }
        }

        private async Task<string> SaveProductImageAsync(IFormFile image)
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, PNG, GIF, and WebP files are allowed.");
            }

            // Validate file size (max 5MB)
            if (image.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentException("File size cannot exceed 5MB.");
            }

            // Create uploads directory if it doesn't exist
            var webRootPath = _environment.WebRootPath ?? Directory.GetCurrentDirectory();
            var uploadsPath = Path.Combine(webRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return relative URL
            return $"/uploads/products/{fileName}";
        }

        private async Task DeleteProductImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                // Extract filename from URL
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                // Log error but don't throw - image deletion shouldn't fail the main operation
            }
        }
    }

    // Request DTOs
    public class CreateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public int CategoryId { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty; // "KG", "G", "ML", "L", "PCS", "BOX", "SET", "UNIT", "DOZEN", "PACK", "ROLL"
        public IFormFile? Image { get; set; }
    }

    public class UpdateProductRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty; // "KG", "G", "ML", "L", "PCS", "BOX", "SET", "UNIT", "DOZEN", "PACK", "ROLL"
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public IFormFile? Image { get; set; }
    }

    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty; // "IN", "OUT", "ADJUSTMENT"
    }
}