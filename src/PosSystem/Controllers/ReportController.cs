using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Attributes;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("daily-sales")]
        public async Task<IActionResult> GetDailySalesReport([FromQuery] DateTime date, [FromQuery] int? storeId = null)
        {
            try
            {
                var report = await _reportService.GetDailySalesReportAsync(date, storeId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating daily sales report", error = ex.Message });
            }
        }

        [HttpGet("product-sales")]
        public async Task<IActionResult> GetProductSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var report = await _reportService.GetProductSalesReportAsync(startDate, endDate, storeId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating product sales report", error = ex.Message });
            }
        }

        [HttpGet("cashier-performance")]
        public async Task<IActionResult> GetCashierPerformanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var report = await _reportService.GetCashierPerformanceReportAsync(startDate, endDate, storeId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating cashier performance report", error = ex.Message });
            }
        }

        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProductsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null, [FromQuery] int topCount = 10)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                if (topCount <= 0 || topCount > 100)
                {
                    return BadRequest(new { message = "Top count must be between 1 and 100" });
                }

                var report = await _reportService.GetTopSellingProductsReportAsync(startDate, endDate, storeId, topCount);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating top selling products report", error = ex.Message });
            }
        }

        [HttpGet("profit-margin")]
        public async Task<IActionResult> GetProfitMarginReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var report = await _reportService.GetProfitMarginReportAsync(startDate, endDate, storeId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating profit margin report", error = ex.Message });
            }
        }

        [HttpGet("stock-movement")]
        public async Task<IActionResult> GetStockMovementReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? productId = null)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var report = await _reportService.GetStockMovementReportAsync(startDate, endDate, productId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating stock movement report", error = ex.Message });
            }
        }

        [HttpGet("sales-summary")]
        public async Task<IActionResult> GetSalesSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                // Get multiple reports for a comprehensive summary
                var dailySales = await _reportService.GetDailySalesReportAsync(endDate, storeId);
                var productSales = await _reportService.GetProductSalesReportAsync(startDate, endDate, storeId);
                var topProducts = await _reportService.GetTopSellingProductsReportAsync(startDate, endDate, storeId, 5);
                var profitMargin = await _reportService.GetProfitMarginReportAsync(startDate, endDate, storeId);

                var summary = new
                {
                    DailySales = dailySales,
                    ProductSales = productSales,
                    TopProducts = topProducts,
                    ProfitMargin = profitMargin
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating sales summary", error = ex.Message });
            }
        }

        [HttpGet("dashboard-metrics")]
        public async Task<IActionResult> GetDashboardMetrics([FromQuery] int? storeId = null)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                startOfMonth = startOfMonth.ToUniversalTime();
                
                // Get today's sales and this month's data
                var todaySales = await _reportService.GetDailySalesReportAsync(today, storeId);
                var monthSales = await _reportService.GetProductSalesReportAsync(startOfMonth, today, storeId);
                var topProducts = await _reportService.GetTopSellingProductsReportAsync(startOfMonth, today, storeId, 5);

                var metrics = new
                {
                    TodaySales = todaySales,
                    MonthSales = monthSales,
                    TopProducts = topProducts,
                    Period = new { StartDate = startOfMonth, EndDate = today }
                };

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating dashboard metrics", error = ex.Message });
            }
        }
    }
}