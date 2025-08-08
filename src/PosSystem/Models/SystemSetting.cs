using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        public string SettingValue { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }
        public User? UpdatedByUser { get; set; }
    }
}