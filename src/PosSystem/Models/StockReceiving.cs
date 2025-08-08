using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class StockReceiving
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ReceivingNumber { get; set; } = string.Empty;

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public int? StoreId { get; set; }
        public Store? Store { get; set; }

        public DateTime ReceivingDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? SupplierInvoiceNumber { get; set; }

        [StringLength(100)]
        public string? DeliveryNote { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "DRAFT"; // DRAFT, RECEIVED, POSTED, CANCELLED

        public decimal TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }

        public string? Notes { get; set; }
        public string? QualityNotes { get; set; }

        public int ReceivedBy { get; set; }
        public User ReceivedByUser { get; set; } = null!;

        public int? PostedBy { get; set; }
        public User? PostedByUser { get; set; }
        public DateTime? PostedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StockReceivingItem> StockReceivingItems { get; set; } = new List<StockReceivingItem>();
    }
}