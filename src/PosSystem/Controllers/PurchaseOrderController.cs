using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PosSystem.Models;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAllPurchaseOrders()
        {
            var purchaseOrders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
            return Ok(purchaseOrders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            return Ok(purchaseOrder);
        }

        [HttpGet("number/{poNumber}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrderByNumber(string poNumber)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByNumberAsync(poNumber);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            return Ok(purchaseOrder);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersBySupplier(int supplierId)
        {
            var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersBySupplierAsync(supplierId);
            return Ok(purchaseOrders);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersByStatus(string status)
        {
            var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersByStatusAsync(status);
            return Ok(purchaseOrders);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPendingPurchaseOrders()
        {
            var purchaseOrders = await _purchaseOrderService.GetPendingPurchaseOrdersAsync();
            return Ok(purchaseOrders);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder(PurchaseOrderDto purchaseOrder)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            if(!int.TryParse(User.FindFirst("StoreId")?.Value, out int storeId))
            {
                return BadRequest(new { message = "Store ID not found" });
            }

            try
            {
                var newPurchaseOrder = new PurchaseOrder
            {
                PurchaseOrderNumber = await _purchaseOrderService.GeneratePurchaseOrderNumberAsync(),
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                SupplierId = purchaseOrder.SupplierId,
                CreatedBy = userId,
                StoreId = storeId,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate.ToUniversalTime(),
                Notes = purchaseOrder.Notes,
                PurchaseOrderItems  = purchaseOrder.Items.Select(item => new PurchaseOrderItem
                {
                    ProductId = item.ProductId,
                    QuantityOrdered = item.QuantityOrdered,
                    UnitCost = item.UnitCost,

                }).ToList()
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPurchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(newPurchaseOrder);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = createdPurchaseOrder.Id }, createdPurchaseOrder);
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating purchase order", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPurchaseOrder = await _purchaseOrderService.UpdatePurchaseOrderAsync(purchaseOrder);
            if (updatedPurchaseOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedPurchaseOrder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            var result = await _purchaseOrderService.DeletePurchaseOrderAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePurchaseOrder(int id, [FromBody] int approvedBy)
        {
            var result = await _purchaseOrderService.ApprovePurchaseOrderAsync(id, approvedBy);
            if (!result)
            {
                return BadRequest("Cannot approve purchase order");
            }

            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPurchaseOrder(int id)
        {
            var result = await _purchaseOrderService.CancelPurchaseOrderAsync(id);
            if (!result)
            {
                return BadRequest("Cannot cancel purchase order");
            }

            return Ok();
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GeneratePurchaseOrderNumber()
        {
            var poNumber = await _purchaseOrderService.GeneratePurchaseOrderNumberAsync();
            return Ok(poNumber);
        }

        [HttpGet("{id}/total")]
        public async Task<ActionResult<decimal>> CalculatePurchaseOrderTotal(int id)
        {
            var total = await _purchaseOrderService.CalculatePurchaseOrderTotalAsync(id);
            return Ok(total);
        }
    }

    public class PurchaseOrderDto
    {
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public DateTime ExpectedDeliveryDate { get; set; }
        public string Notes { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; }
    }
    public class PurchaseOrderItemDto
    {
        public int ProductId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitCost { get; set; }
    }
}