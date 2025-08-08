using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;
using System.Security.Claims;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSettings()
        {
            try
            {
                var settings = await _systemSettingService.GetAllSettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving system settings", error = ex.Message });
            }
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetSetting(string key)
        {
            try
            {
                var setting = await _systemSettingService.GetSettingAsync(key);
                if (setting == null)
                {
                    return NotFound(new { message = "Setting not found" });
                }
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the setting", error = ex.Message });
            }
        }

        [HttpGet("{key}/value")]
        public async Task<IActionResult> GetSettingValue(string key)
        {
            try
            {
                var value = await _systemSettingService.GetSettingValueAsync(key);
                if (value == null)
                {
                    return NotFound(new { message = "Setting not found" });
                }
                return Ok(new { key, value });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the setting value", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetSetting([FromBody] SetSettingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                await _systemSettingService.SetSettingAsync(request.SettingKey, request.SettingValue, userId, request.Description);
                
                var setting = await _systemSettingService.GetSettingAsync(request.SettingKey);
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while setting the system setting", error = ex.Message });
            }
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var exists = await _systemSettingService.SettingExistsAsync(key);
                if (!exists)
                {
                    return NotFound(new { message = "Setting not found" });
                }

                await _systemSettingService.SetSettingAsync(key, request.SettingValue, userId, request.Description);
                
                var setting = await _systemSettingService.GetSettingAsync(key);
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the system setting", error = ex.Message });
            }
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteSetting(string key)
        {
            try
            {
                var exists = await _systemSettingService.SettingExistsAsync(key);
                if (!exists)
                {
                    return NotFound(new { message = "Setting not found" });
                }

                await _systemSettingService.DeleteSettingAsync(key);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the system setting", error = ex.Message });
            }
        }

        [HttpGet("{key}/exists")]
        public async Task<IActionResult> CheckSettingExists(string key)
        {
            try
            {
                var exists = await _systemSettingService.SettingExistsAsync(key);
                return Ok(new { key, exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking setting existence", error = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> SetMultipleSettings([FromBody] SetMultipleSettingsRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var tasks = request.Settings.Select(setting => 
                    _systemSettingService.SetSettingAsync(setting.Key, setting.Value, userId, setting.Description)
                );

                await Task.WhenAll(tasks);

                var updatedSettings = new List<SystemSetting>();
                foreach (var setting in request.Settings)
                {
                    var updatedSetting = await _systemSettingService.GetSettingAsync(setting.Key);
                    if (updatedSetting != null)
                    {
                        updatedSettings.Add(updatedSetting);
                    }
                }

                return Ok(updatedSettings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while setting multiple system settings", error = ex.Message });
            }
        }

        [HttpGet("categories/{category}")]
        public async Task<IActionResult> GetSettingsByCategory(string category)
        {
            try
            {
                var allSettings = await _systemSettingService.GetAllSettingsAsync();
                var categorySettings = allSettings.Where(s => s.SettingKey.StartsWith($"{category}.", StringComparison.OrdinalIgnoreCase));
                
                return Ok(categorySettings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving settings by category", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class SetSettingRequest
    {
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateSettingRequest
    {
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class SetMultipleSettingsRequest
    {
        public List<SettingItem> Settings { get; set; } = new();
    }

    public class SettingItem
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}