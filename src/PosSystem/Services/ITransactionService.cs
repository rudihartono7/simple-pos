using PosSystem.Models;

namespace PosSystem.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<Transaction?> GetTransactionByNumberAsync(string transactionNumber);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, int? storeId = null);
        Task<Transaction> AddItemToTransactionAsync(int transactionId, TransactionItem item);
        Task<Transaction> RemoveItemFromTransactionAsync(int transactionId, int itemId);
        Task<Transaction> ProcessPaymentAsync(int transactionId, Payment payment, decimal discountAmount = 0);
        Task<Transaction> CompleteTransactionAsync(int transactionId);
        Task<Transaction> HoldTransactionAsync(int transactionId);
        Task<Transaction> ResumeTransactionAsync(int transactionId);
        Task<Transaction> RefundTransactionAsync(int transactionId, string reason, int userId);
        Task<string> GenerateTransactionNumberAsync(int storeId);
        Task<IEnumerable<Transaction>> GetHeldTransactionsAsync(int storeId);
    }
}