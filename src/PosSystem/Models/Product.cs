using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Barcode { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int MinStockLevel { get; set; } = 0;

        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = "PCS";

        public bool HasVariants { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool HasExpiry { get; set; } = false;
        public bool RequiresWeighing { get; set; } = false;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}