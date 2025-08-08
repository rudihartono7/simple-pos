using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetSupplierByCodeAsync(string supplierCode)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierCode == supplierCode);
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm)
        {
            return await _context.Suppliers
                .Where(s => s.SupplierName.Contains(searchTerm) ||
                           s.SupplierCode!.Contains(searchTerm) ||
                           s.ContactPerson!.Contains(searchTerm))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            supplier.CreatedAt = DateTime.UtcNow;
            supplier.UpdatedAt = DateTime.UtcNow;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.FindAsync(supplier.Id);
            if (existingSupplier == null) return null;

            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierCode = supplier.SupplierCode;
            existingSupplier.ContactPerson = supplier.ContactPerson;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Address = supplier.Address;
            existingSupplier.City = supplier.City;
            existingSupplier.PostalCode = supplier.PostalCode;
            existingSupplier.Country = supplier.Country;
            existingSupplier.TaxId = supplier.TaxId;
            existingSupplier.CreditLimit = supplier.CreditLimit;
            existingSupplier.PaymentTermDays = supplier.PaymentTermDays;
            existingSupplier.IsActive = supplier.IsActive;
            existingSupplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingSupplier;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            // Soft delete - just deactivate
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SupplierCodeExistsAsync(string supplierCode)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.SupplierCode == supplierCode);
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }
    }
}