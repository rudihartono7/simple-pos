using PosSystem.Models;

namespace PosSystem.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier?> GetSupplierByCodeAsync(string supplierCode);
        Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<Supplier?> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> SupplierCodeExistsAsync(string supplierCode);
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
    }
}