using Microsoft.AspNetCore.Mvc;
using PosSystem.Models;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockReceivingController : ControllerBase
    {
        private readonly IStockReceivingService _stockReceivingService;

        public StockReceivingController(IStockReceivingService stockReceivingService)
        {
            _stockReceivingService = stockReceivingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockReceiving>>> GetAllStockReceivings()
        {
            var stockReceivings = await _stockReceivingService.GetAllStockReceivingsAsync();
            return Ok(stockReceivings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockReceiving>> GetStockReceiving(int id)
        {
            var stockReceiving = await _stockReceivingService.GetStockReceivingByIdAsync(id);
            if (stockReceiving == null)
            {
                return NotFound();
            }
            return Ok(stockReceiving);
        }

        [HttpGet("number/{receivingNumber}")]
        public async Task<ActionResult<StockReceiving>> GetStockReceivingByNumber(string receivingNumber)
        {
            var stockReceiving = await _stockReceivingService.GetStockReceivingByNumberAsync(receivingNumber);
            if (stockReceiving == null)
            {
                return NotFound();
            }
            return Ok(stockReceiving);
        }

        [HttpGet("purchase-order/{purchaseOrderId}")]
        public async Task<ActionResult<IEnumerable<StockReceiving>>> GetStockReceivingsByPurchaseOrder(int purchaseOrderId)
        {
            var stockReceivings = await _stockReceivingService.GetStockReceivingsByPurchaseOrderAsync(purchaseOrderId);
            return Ok(stockReceivings);
        }

        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<StockReceiving>>> GetStockReceivingsBySupplier(int supplierId)
        {
            var stockReceivings = await _stockReceivingService.GetStockReceivingsBySupplierAsync(supplierId);
            return Ok(stockReceivings);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<StockReceiving>>> GetStockReceivingsByStatus(string status)
        {
            var stockReceivings = await _stockReceivingService.GetStockReceivingsByStatusAsync(status);
            return Ok(stockReceivings);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<StockReceiving>>> GetPendingStockReceivings()
        {
            var stockReceivings = await _stockReceivingService.GetPendingStockReceivingsAsync();
            return Ok(stockReceivings);
        }

        [HttpPost]
        public async Task<ActionResult<StockReceiving>> CreateStockReceiving(StockReceiving stockReceiving)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdStockReceiving = await _stockReceivingService.CreateStockReceivingAsync(stockReceiving);
            return CreatedAtAction(nameof(GetStockReceiving), new { id = createdStockReceiving.Id }, createdStockReceiving);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStockReceiving(int id, StockReceiving stockReceiving)
        {
            if (id != stockReceiving.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedStockReceiving = await _stockReceivingService.UpdateStockReceivingAsync(stockReceiving);
            if (updatedStockReceiving == null)
            {
                return NotFound();
            }

            return Ok(updatedStockReceiving);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockReceiving(int id)
        {
            var result = await _stockReceivingService.DeleteStockReceivingAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteStockReceiving(int id, [FromBody] int completedBy)
        {
            var result = await _stockReceivingService.CompleteStockReceivingAsync(id, completedBy);
            if (!result)
            {
                return BadRequest("Cannot complete stock receiving");
            }

            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelStockReceiving(int id)
        {
            var result = await _stockReceivingService.CancelStockReceivingAsync(id);
            if (!result)
            {
                return BadRequest("Cannot cancel stock receiving");
            }

            return Ok();
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GenerateReceivingNumber()
        {
            var receivingNumber = await _stockReceivingService.GenerateReceivingNumberAsync();
            return Ok(receivingNumber);
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessStockReceiving(int id, [FromBody] List<StockReceivingItem> receivedItems)
        {
            var result = await _stockReceivingService.ProcessStockReceivingAsync(id, receivedItems);
            if (!result)
            {
                return BadRequest("Cannot process stock receiving");
            }

            return Ok();
        }

        [HttpGet("{id}/total")]
        public async Task<ActionResult<decimal>> CalculateReceivingTotal(int id)
        {
            var total = await _stockReceivingService.CalculateReceivingTotalAsync(id);
            return Ok(total);
        }
    }
}