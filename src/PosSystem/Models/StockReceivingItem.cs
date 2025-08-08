using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class StockReceivingItem
    {
        public int Id { get; set; }

        public int StockReceivingId { get; set; }
        public StockReceiving StockReceiving { get; set; } = null!;

        public int? PurchaseOrderItemId { get; set; }
        public PurchaseOrderItem? PurchaseOrderItem { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public decimal QuantityOrdered { get; set; } = 0;
        public decimal QuantityReceived { get; set; }
        public decimal QuantityAccepted { get; set; }
        public decimal QuantityRejected { get; set; } = 0;

        public decimal UnitCost { get; set; }
        public decimal TotalCost => QuantityAccepted * UnitCost;

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(20)]
        public string QualityStatus { get; set; } = "GOOD"; // GOOD, DAMAGED, EXPIRED, REJECTED

        public string? Notes { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}