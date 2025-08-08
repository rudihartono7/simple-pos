using Microsoft.AspNetCore.Mvc;
using PosSystem.Attributes;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrintController : ControllerBase
    {
        private readonly IPrintService _printService;

        public PrintController(IPrintService printService)
        {
            _printService = printService;
        }

        [HttpGet("receipt/{transactionId}")]
        public async Task<IActionResult> GenerateReceipt(int transactionId)
        {
            try
            {
                var pdfStream = await _printService.GenerateReceiptAsync(transactionId);
                return File(pdfStream, "application/pdf", $"receipt_{transactionId}.pdf");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating receipt", error = ex.Message });
            }
        }

        [HttpGet("bill/{transactionId}")]
        public async Task<IActionResult> GenerateBill(int transactionId)
        {
            try
            {
                var pdfStream = await _printService.GenerateBillAsync(transactionId);
                return File(pdfStream, "application/pdf", $"bill_{transactionId}.pdf");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating bill", error = ex.Message });
            }
        }

        [HttpPost("receipts/multiple")]
        public async Task<IActionResult> GenerateMultipleReceipts([FromBody] int[] transactionIds)
        {
            try
            {
                if (transactionIds == null || transactionIds.Length == 0)
                {
                    return BadRequest(new { message = "Transaction IDs are required" });
                }

                var pdfStream = await _printService.GenerateMultipleReceiptsAsync(transactionIds);
                return File(pdfStream, "application/pdf", $"receipts_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating receipts", error = ex.Message });
            }
        }

        [HttpGet("receipt/{transactionId}/html")]
        public async Task<IActionResult> GetReceiptHtml(int transactionId)
        {
            try
            {
                var html = await _printService.GenerateReceiptHtmlAsync(transactionId);
                return Content(html, "text/html");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating receipt HTML", error = ex.Message });
            }
        }

        [HttpGet("bill/{transactionId}/html")]
        public async Task<IActionResult> GetBillHtml(int transactionId)
        {
            try
            {
                var html = await _printService.GenerateBillHtmlAsync(transactionId);
                return Content(html, "text/html");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating bill HTML", error = ex.Message });
            }
        }

        [HttpGet("receipt/{transactionId}/bytes")]
        public async Task<IActionResult> GetReceiptBytes(int transactionId)
        {
            try
            {
                var bytes = await _printService.GenerateReceiptBytesAsync(transactionId);
                return Ok(new { 
                    data = Convert.ToBase64String(bytes),
                    filename = $"receipt_{transactionId}.pdf",
                    contentType = "application/pdf"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating receipt bytes", error = ex.Message });
            }
        }

        [HttpGet("bill/{transactionId}/bytes")]
        public async Task<IActionResult> GetBillBytes(int transactionId)
        {
            try
            {
                var bytes = await _printService.GenerateBillBytesAsync(transactionId);
                return Ok(new { 
                    data = Convert.ToBase64String(bytes),
                    filename = $"bill_{transactionId}.pdf",
                    contentType = "application/pdf"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating bill bytes", error = ex.Message });
            }
        }
    }
}