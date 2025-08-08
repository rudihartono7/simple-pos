using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionNumber { get; set; } = string.Empty;

        public int StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Completed";

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Paid";

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}