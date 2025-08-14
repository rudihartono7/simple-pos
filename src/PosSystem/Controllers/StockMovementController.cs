using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;
using PosSystem.Constants;
using System.Security.Claims;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockMovementController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;

        public StockMovementController(IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }

        [HttpPost]
        public async Task<IActionResult> RecordMovement([FromBody] RecordMovementRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var movement = new StockMovement
                {
                    ProductId = request.ProductId,
                    MovementType = request.MovementType,
                    Quantity = request.Quantity,
                    Notes = request.Notes,
                    UserId = userId,
                    MovementDate = DateTime.UtcNow
                };

                var recordedMovement = await _stockMovementService.RecordMovementAsync(movement);
                return CreatedAtAction(nameof(GetMovementsByProduct), new { productId = recordedMovement.ProductId }, recordedMovement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while recording stock movement", error = ex.Message });
            }
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetMovementsByProduct(int productId)
        {
            try
            {
                var movements = await _stockMovementService.GetMovementsByProductAsync(productId);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movements", error = ex.Message });
            }
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetMovementsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var movements = await _stockMovementService.GetMovementsByDateRangeAsync(startDate, endDate);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movements", error = ex.Message });
            }
        }

        [HttpGet("by-type/{movementType}")]
        public async Task<IActionResult> GetMovementsByType(string movementType)
        {
            try
            {
                // Validate movement type
                if (!MovementTypes.ValidTypes.Contains(movementType.ToUpper()))
                {
                    return BadRequest(new { message = "Invalid movement type. Valid types are: " + string.Join(", ", MovementTypes.ValidTypes) });
                }

                var movements = await _stockMovementService.GetMovementsByTypeAsync(movementType.ToUpper());
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movements", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovements([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] int? productId = null, [FromQuery] string? movementType = null)
        {
            try
            {
                IEnumerable<StockMovement> movements;

                if (productId.HasValue)
                {
                    movements = await _stockMovementService.GetMovementsByProductAsync(productId.Value);
                }
                else if (startDate.HasValue && endDate.HasValue)
                {
                    if (startDate > endDate)
                    {
                        return BadRequest(new { message = "Start date cannot be greater than end date" });
                    }
                    movements = await _stockMovementService.GetMovementsByDateRangeAsync(startDate.Value, endDate.Value);
                }
                else if (!string.IsNullOrEmpty(movementType))
                {
                    if (!MovementTypes.ValidTypes.Contains(movementType.ToUpper()))
                    {
                        return BadRequest(new { message = "Invalid movement type. Valid types are: " + string.Join(", ", MovementTypes.ValidTypes) });
                    }
                    movements = await _stockMovementService.GetMovementsByTypeAsync(movementType.ToUpper());
                }
                else
                {
                    // Default to last 30 days if no filters provided
                    var defaultEndDate = DateTime.UtcNow;
                    var defaultStartDate = defaultEndDate.AddDays(-30);
                    movements = await _stockMovementService.GetMovementsByDateRangeAsync(defaultStartDate, defaultEndDate);
                }

                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock movements", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetMovementSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var actualStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
                var actualEndDate = endDate ?? DateTime.UtcNow;

                if (actualStartDate > actualEndDate)
                {
                    return BadRequest(new { message = "Start date cannot be greater than end date" });
                }

                var movements = await _stockMovementService.GetMovementsByDateRangeAsync(actualStartDate, actualEndDate);
                
                var summary = movements.GroupBy(m => m.MovementType)
                    .Select(g => new
                    {
                        MovementType = g.Key,
                        TotalMovements = g.Count(),
                        TotalQuantity = g.Sum(m => Math.Abs(m.Quantity))
                    })
                    .OrderBy(s => s.MovementType)
                    .ToList();

                var result = new
                {
                    Period = new { StartDate = actualStartDate, EndDate = actualEndDate },
                    TotalMovements = movements.Count(),
                    Summary = summary
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating movement summary", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class RecordMovementRequest
    {
        public int ProductId { get; set; }
        public string MovementType { get; set; } = string.Empty; // "IN", "OUT", "ADJUSTMENT", "TRANSFER", "RETURN"
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }
    }
}