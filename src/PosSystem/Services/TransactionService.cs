using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using PosSystem.Constants;

namespace PosSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockMovementService _stockMovementService;

        public TransactionService(ApplicationDbContext context, IStockMovementService stockMovementService)
        {
            _context = context;
            _stockMovementService = stockMovementService;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.Status = "Pending";
            transaction.PaymentStatus = "Pending";

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Product)
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.ProductVariant)
                .Include(t => t.Payments)
                .Include(t => t.Customer)
                .Include(t => t.User)
                .Include(t => t.Store)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction?> GetTransactionByNumberAsync(string transactionNumber)
        {
            return await _context.Transactions
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Product)
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.ProductVariant)
                .Include(t => t.Payments)
                .Include(t => t.Customer)
                .Include(t => t.User)
                .Include(t => t.Store)
                .FirstOrDefaultAsync(t => t.TransactionNumber == transactionNumber);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, int? storeId = null)
        {
            var query = _context.Transactions
                .Include(t => t.TransactionItems)
                .Include(t => t.Customer)
                .Include(t => t.User)
                .Include(t => t.Payments)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);

            if (storeId.HasValue)
            {
                query = query.Where(t => t.StoreId == storeId.Value);
            }

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction> AddItemToTransactionAsync(int transactionId, TransactionItem item)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            // Check stock availability
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Calculate line total
            item.LineTotal = (item.UnitPrice * item.Quantity) - item.DiscountAmount;

            _context.TransactionItems.Add(item);

            // Update transaction totals
            await UpdateTransactionTotalsAsync(transactionId);

            await _context.SaveChangesAsync();

            return await GetTransactionByIdAsync(transactionId) ?? transaction;
        }

        public async Task<Transaction> RemoveItemFromTransactionAsync(int transactionId, int itemId)
        {
            var item = await _context.TransactionItems.FindAsync(itemId);
            if (item == null || item.TransactionId != transactionId)
                throw new ArgumentException("Transaction item not found");

            _context.TransactionItems.Remove(item);

            // Update transaction totals
            await UpdateTransactionTotalsAsync(transactionId);

            await _context.SaveChangesAsync();

            return await GetTransactionByIdAsync(transactionId) ?? new Transaction();
        }

        public async Task<Transaction> ProcessPaymentAsync(int transactionId, Payment payment, decimal discountAmount = 0)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            payment.TransactionId = transactionId;
            payment.PaymentDate = DateTime.UtcNow;
            transaction.DiscountAmount = discountAmount;

            _context.Payments.Add(payment);

            // Update payment status
            var totalPaid = transaction.Payments.Sum(p => p.Amount) + payment.Amount;
            if (totalPaid >= transaction.TotalAmount)
            {
                transaction.PaymentStatus = "Paid";
            }
            else
            {
                transaction.PaymentStatus = "Partial";
            }

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return await GetTransactionByIdAsync(transactionId) ?? transaction;
        }

        public async Task<Transaction> CompleteTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            if (transaction.PaymentStatus != "Paid")
                throw new InvalidOperationException("Transaction must be fully paid before completion");

            transaction.Status = "Completed";

            // Update stock for each item
            foreach (var item in transaction.TransactionItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= (int)item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;

                    // Record stock movement
                    await _stockMovementService.RecordMovementAsync(new StockMovement
                    {
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        MovementType = MovementTypes.SALE,
                        Quantity = item.Quantity,
                        ReferenceId = transactionId,
                        ReferenceType = "Transaction",
                        UserId = transaction.UserId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> HoldTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            transaction.Status = "Hold";
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> ResumeTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            if (transaction.Status != "Hold")
                throw new InvalidOperationException("Only held transactions can be resumed");

            transaction.Status = "Pending";
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> RefundTransactionAsync(int transactionId, string reason, int userId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            if (transaction.Status != "Completed")
                throw new InvalidOperationException("Only completed transactions can be refunded");

            transaction.Status = "Refunded";
            transaction.Notes = $"Refunded: {reason}";

            // Restore stock for each item
            foreach (var item in transaction.TransactionItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += (int)item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;

                    // Record stock movement
                    await _stockMovementService.RecordMovementAsync(new StockMovement
                    {
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        MovementType = MovementTypes.RETURN,
                        Quantity = item.Quantity,
                        ReferenceId = transactionId,
                        ReferenceType = "Transaction",
                        UserId = userId,
                        Notes = $"Refund: {reason}"
                    });
                }
            }

            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<string> GenerateTransactionNumberAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            var storeCode = store?.StoreCode ?? "STR";
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            
            var lastTransaction = await _context.Transactions
                .Where(t => t.StoreId == storeId && t.TransactionDate.Date == DateTime.UtcNow.Date)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();

            var sequence = 1;
            if (lastTransaction != null)
            {
                var lastNumber = lastTransaction.TransactionNumber;
                if (int.TryParse(lastNumber.Substring(lastNumber.Length - 4), out var lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"{storeCode}-{date}-{sequence:D4}";
        }

        public async Task<IEnumerable<Transaction>> GetHeldTransactionsAsync(int storeId)
        {
            return await _context.Transactions
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Product)
                .Include(t => t.User)
                .Where(t => t.StoreId == storeId && t.Status == "Hold")
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        private async Task UpdateTransactionTotalsAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.TransactionItems)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction != null)
            {
                transaction.SubTotal = transaction.TransactionItems.Sum(ti => ti.LineTotal);
                
                // Calculate tax (assuming tax rate from store)
                var store = await _context.Stores.FindAsync(transaction.StoreId);
                var taxRate = store?.TaxRate ?? 0.11m;
                transaction.TaxAmount = (transaction.SubTotal - transaction.DiscountAmount) * taxRate;
                
                transaction.TotalAmount = transaction.SubTotal - transaction.DiscountAmount + transaction.TaxAmount;
            }
        }
    }
}