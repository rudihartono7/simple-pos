using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string? CustomerCode { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

        [StringLength(20)]
        public string CustomerGroup { get; set; } = "Regular";

        public int LoyaltyPoints { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}