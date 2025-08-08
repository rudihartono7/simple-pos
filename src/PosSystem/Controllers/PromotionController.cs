using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePromotions()
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotionsAsync();
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active promotions", error = ex.Message });
            }
        }

        [HttpGet("by-code/{promotionCode}")]
        public async Task<IActionResult> GetPromotionByCode(string promotionCode)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByCodeAsync(promotionCode);
                if (promotion == null)
                {
                    return NotFound(new { message = "Promotion not found" });
                }
                return Ok(promotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the promotion", error = ex.Message });
            }
        }

        [HttpPost("calculate-discount")]
        public async Task<IActionResult> CalculateDiscount([FromBody] CalculateDiscountRequest request)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByCodeAsync(request.PromotionCode);
                if (promotion == null)
                {
                    return NotFound(new { message = "Promotion not found" });
                }
                var items = request.Items.Select(i => new TransactionItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList();

                var discount = await _promotionService.CalculateDiscountAsync(items, promotion);
                return Ok(new { discount, promotion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating discount", error = ex.Message });
            }
        }

        [HttpPost("applicable")]
        public async Task<IActionResult> GetApplicablePromotions([FromBody] GetApplicablePromotionsRequest request)
        {
            try
            {
                var promotions = await _promotionService.GetApplicablePromotionsAsync(request.Items);
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving applicable promotions", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionRequest request)
        {
            try
            {
                var promotion = new Promotion
                {
                    PromotionCode = request.PromotionCode,
                    PromotionName = request.PromotionName,
                    PromotionType = request.PromotionType,
                    Value = request.Value,
                    MinimumPurchase = request.MinimumPurchase,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ApplicableProducts = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPromotion = await _promotionService.CreatePromotionAsync(promotion);
                return CreatedAtAction(nameof(GetPromotionByCode), new { promotionCode = createdPromotion.PromotionCode }, createdPromotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the promotion", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] UpdatePromotionRequest request)
        {
            try
            {
                // First get the existing promotion to update
                var existingPromotion = await _promotionService.GetPromotionByCodeAsync(request.PromotionCode);
                if (existingPromotion == null || existingPromotion.Id != id)
                {
                    return NotFound(new { message = "Promotion not found" });
                }

                existingPromotion.PromotionCode = request.PromotionCode;
                existingPromotion.PromotionName = request.PromotionName;
                existingPromotion.PromotionType = request.PromotionType;
                existingPromotion.Value = request.Value;
                existingPromotion.MinimumPurchase = request.MinimumPurchase;
                existingPromotion.StartDate = request.StartDate;
                existingPromotion.EndDate = request.EndDate;
                existingPromotion.IsActive = request.IsActive;

                var updatedPromotion = await _promotionService.UpdatePromotionAsync(existingPromotion);
                return Ok(updatedPromotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the promotion", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                var result = await _promotionService.DeletePromotionAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Promotion not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the promotion", error = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePromotion([FromBody] ValidatePromotionRequest request)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByCodeAsync(request.PromotionCode);
                if (promotion == null)
                {
                    return NotFound(new { message = "Promotion not found" });
                }

                var isValid = await _promotionService.ValidatePromotionAsync(promotion, request.Items);
                return Ok(new { isValid, promotion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating the promotion", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CalculateDiscountRequest
    {
        public string PromotionCode { get; set; } = string.Empty;
        public List<TransactionItemRequest> Items { get; set; } = new();
    }

    public class TransactionItemRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class GetApplicablePromotionsRequest
    {
        public List<TransactionItem> Items { get; set; } = new();
    }

    public class CreatePromotionRequest
    {
        public string PromotionCode { get; set; } = string.Empty;
        public string PromotionName { get; set; } = string.Empty;
        public string PromotionType { get; set; } = string.Empty; // "PERCENTAGE", "FIXED_AMOUNT", "BUY_X_GET_Y"
        public decimal Value { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] ApplicableProducts { get; set; } = Array.Empty<string>(); // Product IDs or SKUs
        public int? UsageLimit { get; set; } // Optional usage limit
        public bool IsActive { get; set; } = true; // Default to active
    }

    public class UpdatePromotionRequest
    {
        public string PromotionCode { get; set; } = string.Empty;
        public string PromotionName { get; set; } = string.Empty;
        public string PromotionType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string[] ApplicableProducts { get; set; } = Array.Empty<string>(); // Product IDs or SKUs
        public int? UsageLimit { get; set; } // Optional usage limit
        public bool IsActive { get; set; } = true;
    }

    public class ValidatePromotionRequest
    {
        public string PromotionCode { get; set; } = string.Empty;
        public List<TransactionItem> Items { get; set; } = new();
    }
}