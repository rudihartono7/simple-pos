using System.ComponentModel.DataAnnotations;

namespace PosSystem.Models
{
    public class StockTransferItem
    {
        public int Id { get; set; }

        public int StockTransferId { get; set; }
        public StockTransfer StockTransfer { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public int FromWarehouseStockId { get; set; }
        public WarehouseStock FromWarehouseStock { get; set; } = null!;

        public int ToWarehouseStockId { get; set; }
        public WarehouseStock ToWarehouseStock { get; set; } = null!;

        public decimal QuantityRequested { get; set; }
        public decimal QuantityShipped { get; set; } = 0;
        public decimal QuantityReceived { get; set; } = 0;

        public decimal UnitCost { get; set; }
        public decimal TotalValue => QuantityReceived * UnitCost;

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}