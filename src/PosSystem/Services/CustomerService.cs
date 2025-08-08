using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Customer?> GetCustomerByCodeAsync(string customerCode)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == customerCode && c.IsActive);
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone && c.IsActive);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.CustomerCode))
            {
                customer.CustomerCode = await GenerateCustomerCodeAsync();
            }

            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            customer.UpdatedAt = DateTime.UtcNow;
            
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer> AddLoyaltyPointsAsync(int customerId, int points)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            customer.LoyaltyPoints += points;
            customer.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> RedeemLoyaltyPointsAsync(int customerId, int points)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            if (customer.LoyaltyPoints < points)
                throw new InvalidOperationException("Insufficient loyalty points");

            customer.LoyaltyPoints -= points;
            customer.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<string> GenerateCustomerCodeAsync()
        {
            var lastCustomer = await _context.Customers
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            var sequence = 1;
            if (lastCustomer != null && !string.IsNullOrEmpty(lastCustomer.CustomerCode))
            {
                var lastCode = lastCustomer.CustomerCode;
                if (lastCode.StartsWith("CUST") && int.TryParse(lastCode.Substring(4), out var lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"CUST{sequence:D6}";
        }
    }
}