using Microsoft.AspNetCore.Mvc;
using PosSystem.Models;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return Ok(warehouse);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByCode(string code)
        {
            var warehouse = await _warehouseService.GetWarehouseByCodeAsync(code);
            if (warehouse == null)
            {
                return NotFound();
            }
            return Ok(warehouse);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetActiveWarehouses()
        {
            var warehouses = await _warehouseService.GetActiveWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("type/{warehouseType}")]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehousesByType(string warehouseType)
        {
            var warehouses = await _warehouseService.GetWarehousesByTypeAsync(warehouseType);
            return Ok(warehouses);
        }

        [HttpPost]
        public async Task<ActionResult<Warehouse>> CreateWarehouse(Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if warehouse code already exists
            if (await _warehouseService.WarehouseCodeExistsAsync(warehouse.WarehouseCode))
            {
                return BadRequest("Warehouse code already exists");
            }

            var createdWarehouse = await _warehouseService.CreateWarehouseAsync(warehouse);
            return CreatedAtAction(nameof(GetWarehouse), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedWarehouse = await _warehouseService.UpdateWarehouseAsync(warehouse);
            if (updatedWarehouse == null)
            {
                return NotFound();
            }

            return Ok(updatedWarehouse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            if (!result)
            {
                return BadRequest("Cannot delete warehouse with existing stock");
            }

            return NoContent();
        }

        [HttpGet("check-code/{code}")]
        public async Task<ActionResult<bool>> CheckWarehouseCodeExists(string code)
        {
            var exists = await _warehouseService.WarehouseCodeExistsAsync(code);
            return Ok(exists);
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<IEnumerable<WarehouseStock>>> GetWarehouseStock(int id)
        {
            var stock = await _warehouseService.GetWarehouseStockAsync(id);
            return Ok(stock);
        }

        [HttpGet("{warehouseId}/stock/product/{productId}")]
        public async Task<ActionResult<WarehouseStock>> GetWarehouseStockByProduct(int warehouseId, int productId, [FromQuery] int? productVariantId = null)
        {
            var stock = await _warehouseService.GetWarehouseStockByProductAsync(warehouseId, productId, productVariantId);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock);
        }

        [HttpPost("{warehouseId}/stock/update")]
        public async Task<IActionResult> UpdateWarehouseStock(int warehouseId, [FromBody] UpdateWarehouseStockRequest request)
        {
            var result = await _warehouseService.UpdateWarehouseStockAsync(
                warehouseId, 
                request.ProductId, 
                request.ProductVariantId, 
                request.Quantity, 
                request.MovementType);
            
            if (!result)
            {
                return BadRequest("Failed to update warehouse stock");
            }

            return Ok();
        }

        [HttpGet("{id}/low-stock")]
        public async Task<ActionResult<IEnumerable<WarehouseStock>>> GetLowStockItems(int id)
        {
            var lowStockItems = await _warehouseService.GetLowStockItemsAsync(id);
            return Ok(lowStockItems);
        }
    }

    public class UpdateWarehouseStockRequest
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty;
    }
}