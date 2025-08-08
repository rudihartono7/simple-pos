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
        public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPurchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrder);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = createdPurchaseOrder.Id }, createdPurchaseOrder);
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
}