using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IReportService
    {
        Task<DailySalesReport> GetDailySalesReportAsync(DateTime date, int? storeId = null);
        Task<List<ProductSalesReport>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate, int? storeId = null);
        Task<List<CashierPerformanceReport>> GetCashierPerformanceReportAsync(DateTime startDate, DateTime endDate, int? storeId = null);
        Task<List<TopSellingProductReport>> GetTopSellingProductsReportAsync(DateTime startDate, DateTime endDate, int? storeId = null, int topCount = 10);
        Task<ProfitMarginReport> GetProfitMarginReportAsync(DateTime startDate, DateTime endDate, int? storeId = null);
        Task<List<StockMovementReport>> GetStockMovementReportAsync(DateTime startDate, DateTime endDate, int? productId = null);
    }

    public class DailySalesReport
    {
        public DateTime Date { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageTransactionValue { get; set; }
    }

    public class ProductSalesReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class CashierPerformanceReport
    {
        public int UserId { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public int TotalTransactions { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public int TotalItemsSold { get; set; }
    }

    public class TopSellingProductReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int Rank { get; set; }
    }

    public class ProfitMarginReport
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal ProfitMarginPercentage { get; set; }
        public List<ProductProfitMargin> ProductProfitMargins { get; set; } = new List<ProductProfitMargin>();
    }

    public class ProductProfitMargin
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public decimal MarginPercentage { get; set; }
    }

    public class StockMovementReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}