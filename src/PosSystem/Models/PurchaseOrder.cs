using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public int? StoreId { get; set; }
        public Store? Store { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "DRAFT"; // DRAFT, PENDING, APPROVED, ORDERED, PARTIAL_RECEIVED, RECEIVED, CANCELLED

        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }
        public string? Terms { get; set; }

        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public int? ApprovedBy { get; set; }
        public User? ApprovedByUser { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public ICollection<StockReceiving> StockReceivings { get; set; } = new List<StockReceiving>();
    }
}