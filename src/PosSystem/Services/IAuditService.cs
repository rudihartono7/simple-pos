using PosSystem.Models;
using System.Net;

namespace PosSystem.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(int userId, string action, string? tableName = null, int? recordId = null, 
            object? oldValues = null, object? newValues = null, IPAddress? ipAddress = null, string? userAgent = null);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<AuditLog>> GetAuditLogsByTableAsync(string tableName, int? recordId = null);
    }
}