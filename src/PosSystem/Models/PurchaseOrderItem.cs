using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public decimal QuantityOrdered { get; set; }
        public decimal QuantityReceived { get; set; } = 0;
        public decimal QuantityPending => QuantityOrdered - QuantityReceived;

        public decimal UnitCost { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalCost => (QuantityOrdered * UnitCost) - DiscountAmount;

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StockReceivingItem> StockReceivingItems { get; set; } = new List<StockReceivingItem>();
    }
}