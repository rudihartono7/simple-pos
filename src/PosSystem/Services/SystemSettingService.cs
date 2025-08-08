using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using System.Text.Json;

namespace PosSystem.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetSettingAsync(string key)
        {
            return await _context.SystemSettings
                .Include(s => s.UpdatedByUser)
                .FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task<string?> GetSettingValueAsync(string key)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);
            
            return setting?.SettingValue;
        }

        public async Task<T?> GetSettingValueAsync<T>(string key) where T : class
        {
            var settingValue = await GetSettingValueAsync(key);
            
            if (string.IsNullOrEmpty(settingValue))
                return null;

            try
            {
                return JsonSerializer.Deserialize<T>(settingValue);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            return await _context.SystemSettings
                .Include(s => s.UpdatedByUser)
                .OrderBy(s => s.SettingKey)
                .ToListAsync();
        }

        public async Task SetSettingAsync(string key, string value, int updatedBy, string? description = null)
        {
            var existingSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);

            if (existingSetting != null)
            {
                existingSetting.SettingValue = value;
                existingSetting.Description = description ?? existingSetting.Description;
                existingSetting.UpdatedBy = updatedBy;
                existingSetting.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newSetting = new SystemSetting
                {
                    SettingKey = key,
                    SettingValue = value,
                    Description = description,
                    UpdatedBy = updatedBy,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SystemSettings.Add(newSetting);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetSettingAsync<T>(string key, T value, int updatedBy, string? description = null) where T : class
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await SetSettingAsync(key, jsonValue, updatedBy, description);
        }

        public async Task DeleteSettingAsync(string key)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingKey == key);

            if (setting != null)
            {
                _context.SystemSettings.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SettingExistsAsync(string key)
        {
            return await _context.SystemSettings
                .AnyAsync(s => s.SettingKey == key);
        }
    }
}