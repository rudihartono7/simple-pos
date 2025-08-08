using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PromotionName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PromotionCode { get; set; }

        [Required]
        [StringLength(20)]
        public string PromotionType { get; set; } = string.Empty;

        public decimal Value { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public string? ApplicableProducts { get; set; } // JSON array of product IDs
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}