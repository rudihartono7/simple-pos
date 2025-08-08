using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IPrintService
    {
        Task<Stream> GenerateReceiptAsync(int transactionId);
        Task<Stream> GenerateBillAsync(int transactionId);
        Task<Stream> GenerateMultipleReceiptsAsync(int[] transactionIds);
        Task<byte[]> GenerateReceiptBytesAsync(int transactionId);
        Task<byte[]> GenerateBillBytesAsync(int transactionId);
        Task<string> GenerateReceiptHtmlAsync(int transactionId);
        Task<string> GenerateBillHtmlAsync(int transactionId);
    }

    public class ReceiptData
    {
        public required string TransactionNumber { get; set; }
        public required string StoreName { get; set; }
        public required string StoreAddress { get; set; }
        public required string StorePhone { get; set; }
        public required string StoreEmail { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required string CashierName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public required List<ReceiptItem> Items { get; set; }
        public required decimal SubTotal { get; set; }
        public required decimal TaxAmount { get; set; }
        public required decimal DiscountAmount { get; set; }
        public required decimal TotalAmount { get; set; }
        public required decimal ReceivedAmount { get; set; }
        public required decimal ChangeAmount { get; set; }
        public required string PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public required string BarcodeBase64 { get; set; }
        public string? ReceiptFooter { get; set; }
    }

    public class ReceiptItem
    {
        public required string ProductName { get; set; }
        public required string ProductCode { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal LineTotal { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}