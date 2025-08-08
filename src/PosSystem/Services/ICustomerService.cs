using PosSystem.Models;

namespace PosSystem.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByCodeAsync(string customerCode);
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<Customer> AddLoyaltyPointsAsync(int customerId, int points);
        Task<Customer> RedeemLoyaltyPointsAsync(int customerId, int points);
        Task<string> GenerateCustomerCodeAsync();
    }
}