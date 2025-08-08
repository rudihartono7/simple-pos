using Microsoft.AspNetCore.Mvc;
using PosSystem.Services;
using PosSystem.Models;
using PosSystem.Attributes;
using System.Security.Claims;
using System.Net;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(userId, startDate, endDate);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs", error = ex.Message });
            }
        }

        [HttpGet("table/{tableName}")]
        public async Task<IActionResult> GetAuditLogsByTable(string tableName, [FromQuery] int? recordId = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsByTableAsync(tableName, recordId);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs by table", error = ex.Message });
            }
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogAction([FromBody] LogActionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                // Get client IP address
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                
                // Get user agent
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                await _auditService.LogActionAsync(
                    userId,
                    request.Action,
                    request.TableName,
                    request.RecordId,
                    request.OldValues,
                    request.NewValues,
                    ipAddress,
                    userAgent
                );

                return Ok(new { message = "Action logged successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while logging the action", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAuditLogs(
            int userId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(userId, startDate, endDate);
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user audit logs", error = ex.Message });
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentAuditLogs([FromQuery] int limit = 50)
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-7); // Last 7 days by default
                
                var auditLogs = await _auditService.GetAuditLogsAsync(null, startDate, endDate);
                var recentLogs = auditLogs.OrderByDescending(a => a.CreatedAt).Take(limit);
                
                return Ok(recentLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving recent audit logs", error = ex.Message });
            }
        }

        [HttpGet("actions")]
        public async Task<IActionResult> GetAuditLogsByAction([FromQuery] string action, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(null, startDate, endDate);
                var filteredLogs = auditLogs.Where(a => a.Action.Contains(action, StringComparison.OrdinalIgnoreCase));
                
                return Ok(filteredLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs by action", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAuditSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var auditLogs = await _auditService.GetAuditLogsAsync(null, startDate, endDate);
                var summary = new
                {
                    TotalLogs = auditLogs.Count(),
                    UniqueUsers = auditLogs.Select(a => a.UserId).Distinct().Count(),
                    ActionSummary = auditLogs.GroupBy(a => a.Action)
                        .Select(g => new { Action = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count),
                    TableSummary = auditLogs.Where(a => !string.IsNullOrEmpty(a.TableName))
                        .GroupBy(a => a.TableName)
                        .Select(g => new { Table = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count),
                    DateRange = new
                    {
                        StartDate = startDate ?? auditLogs.Min(a => a.CreatedAt),
                        EndDate = endDate ?? auditLogs.Max(a => a.CreatedAt)
                    }
                };
                
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating audit summary", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class LogActionRequest
    {
        public string Action { get; set; } = string.Empty;
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public object? OldValues { get; set; }
        public object? NewValues { get; set; }
    }
}