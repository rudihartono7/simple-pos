using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Attributes;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                // Get counts for products, transactions, and customers
                var productsCount = await _context.Products.CountAsync();
                var transactionsCount = await _context.Transactions.CountAsync();
                var customersCount = await _context.Customers.CountAsync();

                var stats = new
                {
                    products = productsCount,
                    transactions = transactionsCount,
                    customers = customersCount
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching dashboard stats", error = ex.Message });
            }
        }
    }
}