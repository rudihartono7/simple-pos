using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Store
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StoreName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string StoreCode { get; set; } = string.Empty;

        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public decimal TaxRate { get; set; } = 0.11m;

        [StringLength(3)]
        public string Currency { get; set; } = "IDR";

        [StringLength(50)]
        public string Timezone { get; set; } = "Asia/Jakarta";

        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}