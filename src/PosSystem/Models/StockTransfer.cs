using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class StockTransfer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransferNumber { get; set; } = string.Empty;

        public int FromWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; } = null!;

        public int ToWarehouseId { get; set; }
        public Warehouse ToWarehouse { get; set; } = null!;

        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public DateTime? ShippedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "DRAFT"; // DRAFT, PENDING, SHIPPED, RECEIVED, CANCELLED

        [Required]
        [StringLength(20)]
        public string TransferType { get; set; } = "REGULAR"; // REGULAR, EMERGENCY, RETURN

        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }

        public string? Notes { get; set; }
        public string? ShippingNotes { get; set; }

        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public int? ShippedBy { get; set; }
        public User? ShippedByUser { get; set; }

        public int? ReceivedBy { get; set; }
        public User? ReceivedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();
    }
}