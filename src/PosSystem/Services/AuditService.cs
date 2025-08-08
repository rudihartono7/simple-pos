using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using System.Net;
using System.Text.Json;

namespace PosSystem.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int userId, string action, string? tableName = null, int? recordId = null, 
            object? oldValues = null, object? newValues = null, IPAddress? ipAddress = null, string? userAgent = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.AuditLogs
                .Include(al => al.User)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(al => al.UserId == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(al => al.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(al => al.CreatedAt <= endDate.Value);
            }

            return await query
                .OrderByDescending(al => al.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByTableAsync(string tableName, int? recordId = null)
        {
            var query = _context.AuditLogs
                .Include(al => al.User)
                .Where(al => al.TableName == tableName);

            if (recordId.HasValue)
            {
                query = query.Where(al => al.RecordId == recordId.Value);
            }

            return await query
                .OrderByDescending(al => al.CreatedAt)
                .ToListAsync();
        }
    }
}