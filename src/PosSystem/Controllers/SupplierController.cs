using Microsoft.AspNetCore.Mvc;
using PosSystem.Models;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Supplier>> GetSupplierByCode(string code)
        {
            var supplier = await _supplierService.GetSupplierByCodeAsync(code);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchSuppliers([FromQuery] string searchTerm)
        {
            var suppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
            return Ok(suppliers);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetActiveSuppliers()
        {
            var suppliers = await _supplierService.GetActiveSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if supplier code already exists
            if (await _supplierService.SupplierCodeExistsAsync(supplier.SupplierCode))
            {
                return BadRequest("Supplier code already exists");
            }

            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.Id }, createdSupplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedSupplier = await _supplierService.UpdateSupplierAsync(supplier);
            if (updatedSupplier == null)
            {
                return NotFound();
            }

            return Ok(updatedSupplier);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("check-code/{code}")]
        public async Task<ActionResult<bool>> CheckSupplierCodeExists(string code)
        {
            var exists = await _supplierService.SupplierCodeExistsAsync(code);
            return Ok(exists);
        }
    }
}