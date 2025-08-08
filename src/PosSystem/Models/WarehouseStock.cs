using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class WarehouseStock
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public decimal QuantityOnHand { get; set; } = 0;
        public decimal QuantityReserved { get; set; } = 0;
        public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;

        public decimal MinStockLevel { get; set; } = 0;
        public decimal MaxStockLevel { get; set; } = 0;
        public decimal ReorderPoint { get; set; } = 0;

        [StringLength(50)]
        public string? Location { get; set; } // Aisle, Shelf, Bin location

        public decimal AverageCost { get; set; } = 0;
        public decimal LastCost { get; set; } = 0;

        public DateTime LastMovementDate { get; set; } = DateTime.UtcNow;
        public DateTime LastCountDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StockTransferItem> StockTransferItemsFrom { get; set; } = new List<StockTransferItem>();
        public ICollection<StockTransferItem> StockTransferItemsTo { get; set; } = new List<StockTransferItem>();
    }
}