using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;
using System.Security.Claims;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IPromotionService _promotionService;

        public TransactionController(ITransactionService transactionService, IPromotionService promotionService)
        {
            _transactionService = transactionService;
            _promotionService = promotionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var transactionNumber = await _transactionService.GenerateTransactionNumberAsync(request.StoreId);

                var transaction = new Transaction
                {
                    TransactionNumber = transactionNumber,
                    StoreId = request.StoreId,
                    CustomerId = request.CustomerId,
                    UserId = userId,
                    Status = "PENDING",
                    SubTotal = 0,
                    TaxAmount = 0,
                    DiscountAmount = 0,
                    TotalAmount = 0,
                    CreatedAt = DateTime.UtcNow
                };

                var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
                return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the transaction", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the transaction", error = ex.Message });
            }
        }

        [HttpGet("by-number/{transactionNumber}")]
        public async Task<IActionResult> GetTransactionByNumber(string transactionNumber)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByNumberAsync(transactionNumber);
                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the transaction", error = ex.Message });
            }
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? storeId = null)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate.AddDays(1), storeId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving transactions", error = ex.Message });
            }
        }

        [HttpGet("held/{storeId}")]
        public async Task<IActionResult> GetHeldTransactions(int storeId)
        {
            try
            {
                var transactions = await _transactionService.GetHeldTransactionsAsync(storeId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving held transactions", error = ex.Message });
            }
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItemToTransaction(int id, [FromBody] AddTransactionItemRequest request)
        {
            try
            {
                var item = new TransactionItem
                {
                    TransactionId = id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    LineTotal = request.UnitPrice * request.Quantity,
                    DiscountAmount = request.DiscountAmount ?? 0
                };

                var transaction = await _transactionService.AddItemToTransactionAsync(id, item);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding item to transaction", error = ex.Message });
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromTransaction(int id, int itemId)
        {
            try
            {
                var transaction = await _transactionService.RemoveItemFromTransactionAsync(id, itemId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing item from transaction", error = ex.Message });
            }
        }

        [HttpPost("{id}/payment")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentRequest request)
        {
            try
            {
                var payment = new Payment
                {
                    TransactionId = id,
                    PaymentMethod = request.PaymentMethod,
                    Amount = request.Amount,
                    ReceivedAmount = request.ReceivedAmount,
                    ChangeAmount = request.ChangeAmount,
                    PaymentDate = DateTime.UtcNow,
                    Status = "COMPLETED"
                };

                var transaction = await _transactionService.ProcessPaymentAsync(id, payment, request.DiscountAmount);
                await _promotionService.ApplyPromotionToTransactionAsync(request.PromotionCode);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing payment", error = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.CompleteTransactionAsync(id);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while completing the transaction", error = ex.Message });
            }
        }

        [HttpPost("{id}/hold")]
        public async Task<IActionResult> HoldTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.HoldTransactionAsync(id);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while holding the transaction", error = ex.Message });
            }
        }

        [HttpPost("{id}/resume")]
        public async Task<IActionResult> ResumeTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.ResumeTransactionAsync(id);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resuming the transaction", error = ex.Message });
            }
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RefundTransaction(int id, [FromBody] RefundTransactionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var transaction = await _transactionService.RefundTransactionAsync(id, request.Reason, userId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing refund", error = ex.Message });
            }
        }

        [HttpGet("generate-number/{storeId}")]
        public async Task<IActionResult> GenerateTransactionNumber(int storeId)
        {
            try
            {
                var transactionNumber = await _transactionService.GenerateTransactionNumberAsync(storeId);
                return Ok(new { transactionNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating transaction number", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateTransactionRequest
    {
        public int StoreId { get; set; }
        public int? CustomerId { get; set; }
    }

    public class AddTransactionItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
    }

    public class ProcessPaymentRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public string PromotionCode { get; set; } = string.Empty;
    }

    public class RefundTransactionRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}