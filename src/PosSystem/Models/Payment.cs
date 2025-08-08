using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public decimal? ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; } = 0;

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Status { get; set; } = "Success";
    }
}