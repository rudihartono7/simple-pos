using Microsoft.EntityFrameworkCore;
using PosSystem.Data;

namespace PosSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DailySalesReport> GetDailySalesReportAsync(DateTime date, int? storeId = null)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(2);

            var query = _context.Transactions
                .Where(t => t.TransactionDate >= startDate && 
                           t.TransactionDate < endDate && 
                           t.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(t => t.StoreId == storeId.Value);
            }

            var transactions = await query.ToListAsync();

            return new DailySalesReport
            {
                Date = date,
                TotalTransactions = transactions.Count,
                TotalSales = transactions.Sum(t => t.TotalAmount),
                TotalTax = transactions.Sum(t => t.TaxAmount),
                TotalDiscount = transactions.Sum(t => t.DiscountAmount),
                TotalItemsSold = await GetTotalItemsSoldAsync(startDate, endDate, storeId),
                AverageTransactionValue = transactions.Any() ? transactions.Average(t => t.TotalAmount) : 0
            };
        }

        public async Task<List<ProductSalesReport>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate, int? storeId = null)
        {
            var query = _context.TransactionItems
                .Include(ti => ti.Product)
                .Include(ti => ti.Transaction)
                .Where(ti => ti.Transaction.TransactionDate >= startDate && 
                            ti.Transaction.TransactionDate <= endDate && 
                            ti.Transaction.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(ti => ti.Transaction.StoreId == storeId.Value);
            }

            var items = await query.ToListAsync();

            return items
                .GroupBy(ti => new { ti.ProductId, ti.Product.ProductName, ti.Product.ProductCode })
                .Select(g => new ProductSalesReport
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductCode = g.Key.ProductCode,
                    QuantitySold = (int)g.Sum(ti => ti.Quantity),
                    TotalRevenue = g.Sum(ti => ti.LineTotal),
                    AveragePrice = g.Average(ti => ti.UnitPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .ToList();
        }

        public async Task<List<CashierPerformanceReport>> GetCashierPerformanceReportAsync(DateTime startDate, DateTime endDate, int? storeId = null)
        {
            var query = _context.Transactions
                .Include(t => t.User)
                .Include(t => t.TransactionItems)
                .Where(t => t.TransactionDate >= startDate && 
                           t.TransactionDate <= endDate && 
                           t.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(t => t.StoreId == storeId.Value);
            }

            var transactions = await query.ToListAsync();

            return transactions
                .GroupBy(t => new { t.UserId, CashierName = $"{t.User.FirstName} {t.User.LastName}" })
                .Select(g => new CashierPerformanceReport
                {
                    UserId = g.Key.UserId,
                    CashierName = g.Key.CashierName,
                    TotalTransactions = g.Count(),
                    TotalSales = g.Sum(t => t.TotalAmount),
                    AverageTransactionValue = g.Average(t => t.TotalAmount),
                    TotalItemsSold = g.Sum(t => t.TransactionItems.Sum(ti => (int)ti.Quantity))
                })
                .OrderByDescending(c => c.TotalSales)
                .ToList();
        }

        public async Task<List<TopSellingProductReport>> GetTopSellingProductsReportAsync(DateTime startDate, DateTime endDate, int? storeId = null, int topCount = 10)
        {
            var query = _context.TransactionItems
                .Include(ti => ti.Product)
                    .ThenInclude(p => p.Category)
                .Include(ti => ti.Transaction)
                .Where(ti => ti.Transaction.TransactionDate >= startDate && 
                            ti.Transaction.TransactionDate <= endDate && 
                            ti.Transaction.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(ti => ti.Transaction.StoreId == storeId.Value);
            }

            var items = await query.ToListAsync();

            return items
                .GroupBy(ti => new { 
                    ti.ProductId, 
                    ti.Product.ProductName, 
                    CategoryName = ti.Product.Category.CategoryName 
                })
                .Select(g => new TopSellingProductReport
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    QuantitySold = (int)g.Sum(ti => ti.Quantity),
                    TotalRevenue = g.Sum(ti => ti.LineTotal)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(topCount)
                .Select((p, index) => new TopSellingProductReport
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.CategoryName,
                    QuantitySold = p.QuantitySold,
                    TotalRevenue = p.TotalRevenue,
                    Rank = index + 1
                })
                .ToList();
        }

        public async Task<ProfitMarginReport> GetProfitMarginReportAsync(DateTime startDate, DateTime endDate, int? storeId = null)
        {
            var query = _context.TransactionItems
                .Include(ti => ti.Product)
                .Include(ti => ti.Transaction)
                .Where(ti => ti.Transaction.TransactionDate >= startDate && 
                            ti.Transaction.TransactionDate <= endDate && 
                            ti.Transaction.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(ti => ti.Transaction.StoreId == storeId.Value);
            }

            var items = await query.ToListAsync();

            var productProfitMargins = items
                .GroupBy(ti => new { ti.ProductId, ti.Product.ProductName })
                .Select(g => new ProductProfitMargin
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Revenue = g.Sum(ti => ti.LineTotal),
                    Cost = g.Sum(ti => (ti.Product.CostPrice ?? 0) * ti.Quantity),
                    Profit = g.Sum(ti => ti.LineTotal - ((ti.Product.CostPrice ?? 0) * ti.Quantity)),
                    MarginPercentage = g.Sum(ti => ti.LineTotal) > 0 ? 
                        (g.Sum(ti => ti.LineTotal - ((ti.Product.CostPrice ?? 0) * ti.Quantity)) / g.Sum(ti => ti.LineTotal)) * 100 : 0
                })
                .ToList();

            var totalRevenue = productProfitMargins.Sum(p => p.Revenue);
            var totalCost = productProfitMargins.Sum(p => p.Cost);
            var grossProfit = totalRevenue - totalCost;

            return new ProfitMarginReport
            {
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                GrossProfit = grossProfit,
                ProfitMarginPercentage = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0,
                ProductProfitMargins = productProfitMargins.OrderByDescending(p => p.Profit).ToList()
            };
        }

        public async Task<List<StockMovementReport>> GetStockMovementReportAsync(DateTime startDate, DateTime endDate, int? productId = null)
        {
            var query = _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.User)
                .Where(sm => sm.MovementDate >= startDate && sm.MovementDate <= endDate);

            if (productId.HasValue)
            {
                query = query.Where(sm => sm.ProductId == productId.Value);
            }

            var movements = await query.ToListAsync();

            return movements
                .Select(sm => new StockMovementReport
                {
                    ProductId = sm.ProductId,
                    ProductName = sm.Product.ProductName,
                    MovementType = sm.MovementType,
                    Quantity = sm.Quantity,
                    MovementDate = sm.MovementDate,
                    UserName = $"{sm.User.FirstName} {sm.User.LastName}",
                    Notes = sm.Notes
                })
                .OrderByDescending(sm => sm.MovementDate)
                .ToList();
        }

        private async Task<int> GetTotalItemsSoldAsync(DateTime startDate, DateTime endDate, int? storeId)
        {
            var query = _context.TransactionItems
                .Include(ti => ti.Transaction)
                .Where(ti => ti.Transaction.TransactionDate >= startDate && 
                            ti.Transaction.TransactionDate < endDate && 
                            ti.Transaction.Status == "Completed");

            if (storeId.HasValue)
            {
                query = query.Where(ti => ti.Transaction.StoreId == storeId.Value);
            }

            return (int)await query.SumAsync(ti => ti.Quantity);
        }
    }
}