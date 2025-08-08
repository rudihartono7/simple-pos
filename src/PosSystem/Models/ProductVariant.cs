using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string VariantName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VariantValue { get; set; } = string.Empty;

        public decimal? UnitPrice { get; set; }
        public int StockQuantity { get; set; } = 0;

        [StringLength(100)]
        public string? Barcode { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}