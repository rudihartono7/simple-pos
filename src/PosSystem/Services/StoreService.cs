using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationDbContext _context;

        public StoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _context.Stores
                .Where(s => s.IsActive)
                .OrderBy(s => s.StoreName)
                .ToListAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(int id)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<Store?> GetStoreByCodeAsync(string storeCode)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(s => s.StoreCode == storeCode && s.IsActive);
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            if (string.IsNullOrEmpty(store.StoreCode))
            {
                store.StoreCode = await GenerateStoreCodeAsync();
            }

            store.CreatedAt = DateTime.UtcNow;
            store.UpdatedAt = DateTime.UtcNow;
            store.IsActive = true;

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return store;
        }

        public async Task<Store> UpdateStoreAsync(Store store)
        {
            var existingStore = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == store.Id);

            if (existingStore == null)
            {
                throw new ArgumentException("Store not found");
            }

            existingStore.StoreName = store.StoreName;
            existingStore.Address = store.Address;
            existingStore.Phone = store.Phone;
            existingStore.TaxRate = store.TaxRate;
            existingStore.Currency = store.Currency;
            existingStore.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingStore;
        }

        public async Task DeleteStoreAsync(int id)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store != null)
            {
                store.IsActive = false;
                store.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateStoreCodeAsync()
        {
            var lastStore = await _context.Stores
                .Where(s => s.StoreCode.StartsWith("STR"))
                .OrderByDescending(s => s.StoreCode)
                .FirstOrDefaultAsync();

            if (lastStore == null)
            {
                return "STR0001";
            }

            var lastCodeNumber = lastStore.StoreCode.Substring(3);
            if (int.TryParse(lastCodeNumber, out int number))
            {
                return $"STR{(number + 1):D4}";
            }

            return "STR0001";
        }

        public async Task<bool> StoreCodeExistsAsync(string storeCode)
        {
            return await _context.Stores
                .AnyAsync(s => s.StoreCode == storeCode);
        }
    }
}