using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStores()
        {
            try
            {
                var stores = await _storeService.GetAllStoresAsync();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving stores", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound(new { message = "Store not found" });
                }
                return Ok(store);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the store", error = ex.Message });
            }
        }

        [HttpGet("by-code/{storeCode}")]
        public async Task<IActionResult> GetStoreByCode(string storeCode)
        {
            try
            {
                var store = await _storeService.GetStoreByCodeAsync(storeCode);
                if (store == null)
                {
                    return NotFound(new { message = "Store not found" });
                }
                return Ok(store);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the store", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            try
            {
                var store = new Store
                {
                    StoreName = request.StoreName,
                    StoreCode = await _storeService.GenerateStoreCodeAsync(),
                    Address = request.Address,
                    Phone = request.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdStore = await _storeService.CreateStoreAsync(store);
                return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, createdStore);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the store", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreRequest request)
        {
            try
            {
                var existingStore = await _storeService.GetStoreByIdAsync(id);
                if (existingStore == null)
                {
                    return NotFound(new { message = "Store not found" });
                }

                existingStore.StoreName = request.StoreName;
                existingStore.Address = request.Address;
                existingStore.Phone = request.Phone;
                existingStore.TaxRate = request.TaxRate;
                existingStore.Currency = request.Currency;
                existingStore.Latitude = request.Latitude;
                existingStore.Longitude = request.Longitude;
                existingStore.GoogleMapsLink = request.GoogleMapsLink;
                existingStore.IsActive = request.IsActive;

                var updatedStore = await _storeService.UpdateStoreAsync(existingStore);
                return Ok(updatedStore);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the store", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound(new { message = "Store not found" });
                }

                await _storeService.DeleteStoreAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the store", error = ex.Message });
            }
        }

        [HttpGet("generate-code")]
        public async Task<IActionResult> GenerateStoreCode()
        {
            try
            {
                var code = await _storeService.GenerateStoreCodeAsync();
                return Ok(new { storeCode = code });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating store code", error = ex.Message });
            }
        }

        [HttpGet("check-code/{storeCode}")]
        public async Task<IActionResult> CheckStoreCodeExists(string storeCode)
        {
            try
            {
                var exists = await _storeService.StoreCodeExistsAsync(storeCode);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking store code", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateStoreRequest
    {
        public string StoreName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal TaxRate { get; set; } = 0.11m;
        public string Currency { get; set; } = "IDR";
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStoreRequest
    {
        public string StoreName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal TaxRate { get; set; } = 0.11m;
        public string Currency { get; set; } = "IDR";
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public bool IsActive { get; set; } = true;
    }
}