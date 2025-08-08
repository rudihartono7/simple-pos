using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? WarehouseCode { get; set; }

        [Required]
        [StringLength(20)]
        public string WarehouseType { get; set; } = "MAIN"; // MAIN, BRANCH, TRANSIT, VIRTUAL

        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public ICollection<StockTransfer> StockTransfersFrom { get; set; } = new List<StockTransfer>();
        public ICollection<StockTransfer> StockTransfersTo { get; set; } = new List<StockTransfer>();
    }
}