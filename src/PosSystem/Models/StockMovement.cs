using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        [Required]
        [StringLength(20)]
        public string MovementType { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public int? ReferenceId { get; set; }

        [StringLength(20)]
        public string? ReferenceType { get; set; }

        public string? Notes { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    }
}