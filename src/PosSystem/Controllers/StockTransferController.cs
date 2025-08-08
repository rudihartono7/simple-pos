using Microsoft.AspNetCore.Mvc;
using PosSystem.Models;
using PosSystem.Services.Interfaces;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockTransferController : ControllerBase
    {
        private readonly IStockTransferService _stockTransferService;

        public StockTransferController(IStockTransferService stockTransferService)
        {
            _stockTransferService = stockTransferService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockTransfer>>> GetAllStockTransfers()
        {
            try
            {
                var stockTransfers = await _stockTransferService.GetAllStockTransfersAsync();
                return Ok(stockTransfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfers", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockTransfer>> GetStockTransferById(int id)
        {
            try
            {
                var stockTransfer = await _stockTransferService.GetStockTransferByIdAsync(id);
                if (stockTransfer == null)
                    return NotFound(new { message = "Stock transfer not found" });

                return Ok(stockTransfer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the stock transfer", error = ex.Message });
            }
        }

        [HttpGet("by-number/{transferNumber}")]
        public async Task<ActionResult<StockTransfer>> GetStockTransferByNumber(string transferNumber)
        {
            try
            {
                var stockTransfer = await _stockTransferService.GetStockTransferByNumberAsync(transferNumber);
                if (stockTransfer == null)
                    return NotFound(new { message = "Stock transfer not found" });

                return Ok(stockTransfer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the stock transfer", error = ex.Message });
            }
        }

        [HttpGet("by-source-warehouse/{sourceWarehouseId}")]
        public async Task<ActionResult<IEnumerable<StockTransfer>>> GetStockTransfersBySourceWarehouse(int sourceWarehouseId)
        {
            try
            {
                var stockTransfers = await _stockTransferService.GetStockTransfersBySourceWarehouseAsync(sourceWarehouseId);
                return Ok(stockTransfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfers", error = ex.Message });
            }
        }

        [HttpGet("by-destination-warehouse/{destinationWarehouseId}")]
        public async Task<ActionResult<IEnumerable<StockTransfer>>> GetStockTransfersByDestinationWarehouse(int destinationWarehouseId)
        {
            try
            {
                var stockTransfers = await _stockTransferService.GetStockTransfersByDestinationWarehouseAsync(destinationWarehouseId);
                return Ok(stockTransfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfers", error = ex.Message });
            }
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<StockTransfer>>> GetStockTransfersByStatus(string status)
        {
            try
            {
                var stockTransfers = await _stockTransferService.GetStockTransfersByStatusAsync(status);
                return Ok(stockTransfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stock transfers", error = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<StockTransfer>>> GetPendingStockTransfers()
        {
            try
            {
                var stockTransfers = await _stockTransferService.GetPendingStockTransfersAsync();
                return Ok(stockTransfers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving pending stock transfers", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<StockTransfer>> CreateStockTransfer([FromBody] StockTransfer stockTransfer)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdStockTransfer = await _stockTransferService.CreateStockTransferAsync(stockTransfer);
                return CreatedAtAction(nameof(GetStockTransferById), new { id = createdStockTransfer.Id }, createdStockTransfer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the stock transfer", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StockTransfer>> UpdateStockTransfer(int id, [FromBody] StockTransfer stockTransfer)
        {
            try
            {
                if (id != stockTransfer.Id)
                    return BadRequest(new { message = "ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedStockTransfer = await _stockTransferService.UpdateStockTransferAsync(stockTransfer);
                return Ok(updatedStockTransfer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the stock transfer", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStockTransfer(int id)
        {
            try
            {
                var result = await _stockTransferService.DeleteStockTransferAsync(id);
                if (!result)
                    return NotFound(new { message = "Stock transfer not found or cannot be deleted" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the stock transfer", error = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<StockTransfer>> ApproveStockTransfer(int id, [FromBody] ApprovalRequest request)
        {
            try
            {
                var approvedStockTransfer = await _stockTransferService.ApproveStockTransferAsync(id, request.ApprovedBy);
                return Ok(approvedStockTransfer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving the stock transfer", error = ex.Message });
            }
        }

        [HttpPost("{id}/ship")]
        public async Task<ActionResult<StockTransfer>> ShipStockTransfer(int id)
        {
            try
            {
                var shippedStockTransfer = await _stockTransferService.ShipStockTransferAsync(id);
                return Ok(shippedStockTransfer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while shipping the stock transfer", error = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<StockTransfer>> CompleteStockTransfer(int id, [FromBody] CompletionRequest request)
        {
            try
            {
                var completedStockTransfer = await _stockTransferService.CompleteStockTransferAsync(id, request.CompletedBy);
                return Ok(completedStockTransfer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while completing the stock transfer", error = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<StockTransfer>> CancelStockTransfer(int id)
        {
            try
            {
                var cancelledStockTransfer = await _stockTransferService.CancelStockTransferAsync(id);
                return Ok(cancelledStockTransfer);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cancelling the stock transfer", error = ex.Message });
            }
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GenerateTransferNumber()
        {
            try
            {
                var transferNumber = await _stockTransferService.GenerateTransferNumberAsync();
                return Ok(new { transferNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating transfer number", error = ex.Message });
            }
        }

        [HttpGet("{id}/calculate-total")]
        public async Task<ActionResult<decimal>> CalculateTransferTotal(int id)
        {
            try
            {
                var total = await _stockTransferService.CalculateTransferTotalAsync(id);
                return Ok(new { total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating transfer total", error = ex.Message });
            }
        }

        [HttpGet("{id}/validate-stock")]
        public async Task<ActionResult<bool>> ValidateStockAvailability(int id)
        {
            try
            {
                var isValid = await _stockTransferService.ValidateStockAvailabilityAsync(id);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating stock availability", error = ex.Message });
            }
        }
    }

    public class ApprovalRequest
    {
        public int ApprovedBy { get; set; }
    }

    public class CompletionRequest
    {
        public int CompletedBy { get; set; }
    }
}