using PosSystem.Models;

namespace PosSystem.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSetting?> GetSettingAsync(string key);
        Task<string?> GetSettingValueAsync(string key);
        Task<T?> GetSettingValueAsync<T>(string key) where T : class;
        Task<IEnumerable<SystemSetting>> GetAllSettingsAsync();
        Task SetSettingAsync(string key, string value, int updatedBy, string? description = null);
        Task SetSettingAsync<T>(string key, T value, int updatedBy, string? description = null) where T : class;
        Task DeleteSettingAsync(string key);
        Task<bool> SettingExistsAsync(string key);
    }
}