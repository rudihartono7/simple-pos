using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving customers", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the customer", error = ex.Message });
            }
        }

        [HttpGet("by-code/{customerCode}")]
        public async Task<IActionResult> GetCustomerByCode(string customerCode)
        {
            try
            {
                var customer = await _customerService.GetCustomerByCodeAsync(customerCode);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the customer", error = ex.Message });
            }
        }

        [HttpGet("by-phone/{phone}")]
        public async Task<IActionResult> GetCustomerByPhone(string phone)
        {
            try
            {
                var customer = await _customerService.GetCustomerByPhoneAsync(phone);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the customer", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customerCode = await _customerService.GenerateCustomerCodeAsync();
                
                var customer = new Customer
                {
                    CustomerCode = customerCode,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    LoyaltyPoints = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the customer", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                var existingCustomer = await _customerService.GetCustomerByIdAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                existingCustomer.FirstName = request.FirstName;
                existingCustomer.LastName = request.LastName;
                existingCustomer.Email = request.Email;
                existingCustomer.Phone = request.Phone;
                existingCustomer.Address = request.Address;
                existingCustomer.DateOfBirth = request.DateOfBirth;
                existingCustomer.IsActive = request.IsActive;

                var updatedCustomer = await _customerService.UpdateCustomerAsync(existingCustomer);
                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the customer", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Customer not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the customer", error = ex.Message });
            }
        }

        [HttpPost("{id}/loyalty-points/add")]
        public async Task<IActionResult> AddLoyaltyPoints(int id, [FromBody] LoyaltyPointsRequest request)
        {
            try
            {
                var customer = await _customerService.AddLoyaltyPointsAsync(id, request.Points);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding loyalty points", error = ex.Message });
            }
        }

        [HttpPost("{id}/loyalty-points/redeem")]
        public async Task<IActionResult> RedeemLoyaltyPoints(int id, [FromBody] LoyaltyPointsRequest request)
        {
            try
            {
                var customer = await _customerService.RedeemLoyaltyPointsAsync(id, request.Points);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while redeeming loyalty points", error = ex.Message });
            }
        }

        [HttpGet("generate-code")]
        public async Task<IActionResult> GenerateCustomerCode()
        {
            try
            {
                var code = await _customerService.GenerateCustomerCodeAsync();
                return Ok(new { customerCode = code });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating customer code", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class LoyaltyPointsRequest
    {
        public int Points { get; set; }
    }
}