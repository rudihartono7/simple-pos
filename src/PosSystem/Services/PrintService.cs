using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using SkiaSharp;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class PrintService : IPrintService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly string _storagePath;

        public PrintService(
            ApplicationDbContext context,
            IHtmlToPdfService htmlToPdfService,
            ISystemSettingService systemSettingService,
            IConfiguration configuration)
        {
            _context = context;
            _htmlToPdfService = htmlToPdfService;
            _systemSettingService = systemSettingService;
            _storagePath = configuration["StoragePath"] ?? "./temp";
        }

        public async Task<Stream> GenerateReceiptAsync(int transactionId)
        {
            var receiptData = await GetReceiptDataAsync(transactionId);
            var htmlContent = await GenerateReceiptHtmlContentAsync(receiptData);
            return await ConvertHtmlToPdfAsync(htmlContent); // Thermal receipt format
        }

        public async Task<Stream> GenerateBillAsync(int transactionId)
        {
            var receiptData = await GetReceiptDataAsync(transactionId);
            var htmlContent = await GenerateBillHtmlContentAsync(receiptData);
            return await ConvertHtmlToPdfAsync(htmlContent); // A4 invoice format
        }

        public async Task<Stream> GenerateMultipleReceiptsAsync(int[] transactionIds)
        {
            var receiptsHtml = new List<string>();
            
            foreach (var transactionId in transactionIds)
            {
                var receiptData = await GetReceiptDataAsync(transactionId);
                var htmlContent = await GenerateReceiptHtmlContentAsync(receiptData);
                receiptsHtml.Add(htmlContent);
            }

            var combinedHtml = string.Join("<div style='page-break-after: always;'></div>", receiptsHtml);
            return await ConvertHtmlToPdfAsync(combinedHtml); // Multiple receipts format
        }

        public async Task<byte[]> GenerateReceiptBytesAsync(int transactionId)
        {
            using var stream = await GenerateReceiptAsync(transactionId);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateBillBytesAsync(int transactionId)
        {
            using var stream = await GenerateBillAsync(transactionId);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<string> GenerateReceiptHtmlAsync(int transactionId)
        {
            var receiptData = await GetReceiptDataAsync(transactionId);
            return await GenerateReceiptHtmlContentAsync(receiptData);
        }

        public async Task<string> GenerateBillHtmlAsync(int transactionId)
        {
            var receiptData = await GetReceiptDataAsync(transactionId);
            return await GenerateBillHtmlContentAsync(receiptData);
        }

        private async Task<ReceiptData> GetReceiptDataAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Store)
                .Include(t => t.Customer)
                .Include(t => t.User)
                .Include(t => t.TransactionItems)
                    .ThenInclude(ti => ti.Product)
                .Include(t => t.Payments)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            var payment = transaction.Payments.FirstOrDefault();
            var barcodeBase64 = GenerateBarcodeBase64(transaction.TransactionNumber);
            var receiptFooter = await _systemSettingService.GetSettingValueAsync("ReceiptFooter");

            return new ReceiptData
            {
                TransactionNumber = transaction.TransactionNumber,
                StoreName = transaction.Store.StoreName,
                StoreAddress = transaction.Store.Address ?? "",
                StorePhone = transaction.Store.Phone ?? "",
                StoreEmail = "", // Store model doesn't have email field
                TransactionDate = transaction.TransactionDate,
                CashierName = $"{transaction.User.FirstName} {transaction.User.LastName}",
                CustomerName = transaction.Customer != null ? $"{transaction.Customer.FirstName} {transaction.Customer.LastName}".Trim() : null,
                CustomerPhone = transaction.Customer?.Phone,
                Items = transaction.TransactionItems.Select(ti => new ReceiptItem
                {
                    ProductName = ti.Product.ProductName,
                    ProductCode = ti.Product.ProductCode,
                    Quantity = (int)ti.Quantity,
                    UnitPrice = ti.UnitPrice,
                    LineTotal = ti.LineTotal,
                    DiscountAmount = ti.DiscountAmount
                }).ToList(),
                SubTotal = transaction.SubTotal,
                TaxAmount = transaction.TaxAmount,
                DiscountAmount = transaction.DiscountAmount,
                TotalAmount = transaction.TotalAmount,
                ReceivedAmount = payment?.ReceivedAmount ?? 0,
                ChangeAmount = payment?.ChangeAmount ?? 0,
                PaymentMethod = payment?.PaymentMethod ?? "Cash",
                Notes = transaction.Notes,
                BarcodeBase64 = barcodeBase64,
                ReceiptFooter = receiptFooter
            };
        }

        private async Task<string> GenerateReceiptHtmlContentAsync(ReceiptData data)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ReceiptTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);
            
            // Replace template variables
            var html = template
                .Replace("{{StoreName}}", data.StoreName)
                .Replace("{{StoreAddress}}", data.StoreAddress)
                .Replace("{{StorePhone}}", data.StorePhone)
                .Replace("{{StoreEmail}}", data.StoreEmail ?? "")
                .Replace("{{TransactionNumber}}", data.TransactionNumber)
                .Replace("{{TransactionDate}}", data.TransactionDate.ToString("dd/MM/yyyy"))
                .Replace("{{TransactionTime}}", data.TransactionDate.ToString("HH:mm:ss"))
                .Replace("{{CashierName}}", data.CashierName)
                .Replace("{{CustomerName}}", data.CustomerName ?? "")
                .Replace("{{CustomerPhone}}", data.CustomerPhone ?? "")
                .Replace("{{BarcodeBase64}}", data.BarcodeBase64)
                .Replace("{{SubtotalFormatted}}", data.SubTotal.ToString("C", new CultureInfo("id-ID")))
                .Replace("{{TotalDiscountFormatted}}", data.DiscountAmount.ToString("C", new CultureInfo("id-ID")))
                .Replace("{{TaxAmountFormatted}}", data.TaxAmount.ToString("C", new CultureInfo("id-ID")))
                .Replace("{{TotalAmountFormatted}}", data.TotalAmount.ToString("C", new CultureInfo("id-ID")))
                .Replace("{{ChangeAmountFormatted}}", data.ChangeAmount.ToString("C", new CultureInfo("id-ID")))
                .Replace("{{FooterText}}", data.ReceiptFooter)
                .Replace("{{PrintDateTime}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            // Handle items
            var itemsHtml = string.Join("", data.Items.Select(item => $@"
        <div class=""item"">
            <div class=""item-name"">{item.ProductName}</div>
            <div class=""item-details"">
                <div>{item.Quantity} x {item.UnitPrice.ToString("C", new CultureInfo("id-ID"))}</div>
                <div class=""item-qty-price"">{item.LineTotal.ToString("C", new CultureInfo("id-ID"))}</div>
            </div>
            {(item.DiscountAmount > 0 ? $@"
            <div class=""item-details"">
                <div>Discount</div>
                <div class=""item-qty-price"">-{item.DiscountAmount.ToString("C", new CultureInfo("id-ID"))}</div>
            </div>" : "")}
        </div>"));

            html = html.Replace("{{#each Items}}", "").Replace("{{/each}}", "");
            var itemsSection = html.Substring(html.IndexOf("<div class=\"items\">"), 
                html.IndexOf("</div>", html.IndexOf("<div class=\"items\">")) - html.IndexOf("<div class=\"items\">") + 6);
            html = html.Replace(itemsSection, $"<div class=\"items\">{itemsHtml}</div>");

            // Handle payments
            var paymentsHtml = $@"
        <div class=""payment-line"">
            <span>{data.PaymentMethod}:</span>
            <span>{data.ReceivedAmount.ToString("C", new CultureInfo("id-ID"))}</span>
        </div>";

            html = html.Replace("{{#each Payments}}", "").Replace("{{/each}}", "");
            var paymentsStart = html.IndexOf("<strong style=\"font-size: 11px;\">Payment Details:</strong>");
            var paymentsEnd = html.IndexOf("{{#if ChangeAmount}}", paymentsStart);
            var paymentsSection = html.Substring(paymentsStart, paymentsEnd - paymentsStart);
            html = html.Replace(paymentsSection, $"<strong style=\"font-size: 11px;\">Payment Details:</strong>{paymentsHtml}");

            // Handle conditional sections
            html = HandleConditionals(html, data);

            return html;
        }

        private async Task<string> GenerateBillHtmlContentAsync(ReceiptData data)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BillTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);
            
            // Replace template variables
            var html = template
                .Replace("{{StoreName}}", data.StoreName)
                .Replace("{{StoreAddress}}", data.StoreAddress)
                .Replace("{{StorePhone}}", data.StorePhone)
                .Replace("{{StoreEmail}}", data.StoreEmail ?? "")
                .Replace("{{TransactionNumber}}", data.TransactionNumber)
                .Replace("{{TransactionDate}}", data.TransactionDate.ToString("dd MMMM yyyy"))
                .Replace("{{TransactionTime}}", data.TransactionDate.ToString("HH:mm"))
                .Replace("{{CashierName}}", data.CashierName)
                .Replace("{{CustomerName}}", data.CustomerName ?? "Walk-in Customer")
                .Replace("{{CustomerPhone}}", data.CustomerPhone ?? "-")
                .Replace("{{BarcodeBase64}}", data.BarcodeBase64)
                .Replace("{{SubtotalFormatted}}", data.SubTotal.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{TotalDiscountFormatted}}", data.DiscountAmount.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{TaxAmountFormatted}}", data.TaxAmount.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{TotalAmountFormatted}}", data.TotalAmount.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{ReceivedAmountFormatted}}", data.ReceivedAmount.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{ChangeAmountFormatted}}", data.ChangeAmount.ToString("C0", new CultureInfo("id-ID")))
                .Replace("{{PaymentMethod}}", data.PaymentMethod)
                .Replace("{{FooterText}}", data.ReceiptFooter ?? "Thank you for your business!")
                .Replace("{{PrintDateTime}}", DateTime.Now.ToString("dd MMMM yyyy HH:mm"));

            // Handle items table
            var itemsHtml = string.Join("", data.Items.Select((item, index) => $@"
            <tr>
                <td>{index + 1}</td>
                <td>{item.ProductCode}</td>
                <td>{item.ProductName}</td>
                <td class='qty'>{item.Quantity}</td>
                <td class='price'>{item.UnitPrice.ToString("C0", new CultureInfo("id-ID"))}</td>
                <td class='total'>{item.LineTotal.ToString("C0", new CultureInfo("id-ID"))}</td>
            </tr>
            {(item.DiscountAmount > 0 ? $@"
            <tr>
                <td colspan='5' style='text-align: right; font-style: italic;'>Item Discount:</td>
                <td class='total' style='font-style: italic;'>-{item.DiscountAmount.ToString("C0", new CultureInfo("id-ID"))}</td>
            </tr>" : "")}
            "));

            html = html.Replace("{{#each Items}}", "").Replace("{{/each}}", "");
            var itemsTableStart = html.IndexOf("<tbody>");
            var itemsTableEnd = html.IndexOf("</tbody>", itemsTableStart);
            var itemsTableSection = html.Substring(itemsTableStart, itemsTableEnd - itemsTableStart + 8);
            html = html.Replace(itemsTableSection, $"<tbody>{itemsHtml}</tbody>");

            // Handle conditional sections
            html = HandleConditionals(html, data);

            return html;
        }

        private async Task<Stream> ConvertHtmlToPdfAsync(string html)
        {
            return await _htmlToPdfService.ConvertHtmlToPdfStreamAsync(html);
        }

        private string HandleConditionals(string html, ReceiptData data)
        {
            // Handle discount conditionals
            if (data.DiscountAmount > 0)
            {
                html = html.Replace("{{#if DiscountAmount}}", "").Replace("{{/if}}", "");
            }
            else
            {
                var discountStart = html.IndexOf("{{#if DiscountAmount}}");
                if (discountStart >= 0)
                {
                    var discountEnd = html.IndexOf("{{/if}}", discountStart) + 7;
                    html = html.Remove(discountStart, discountEnd - discountStart);
                }
            }

            // Handle change amount conditionals
            if (data.ChangeAmount > 0)
            {
                html = html.Replace("{{#if ChangeAmount}}", "").Replace("{{/if}}", "");
            }
            else
            {
                var changeStart = html.IndexOf("{{#if ChangeAmount}}");
                if (changeStart >= 0)
                {
                    var changeEnd = html.IndexOf("{{/if}}", changeStart) + 7;
                    html = html.Remove(changeStart, changeEnd - changeStart);
                }
            }

            // Handle customer name conditionals
            if (!string.IsNullOrEmpty(data.CustomerName))
            {
                html = html.Replace("{{#if CustomerName}}", "").Replace("{{/if}}", "");
            }
            else
            {
                var customerStart = html.IndexOf("{{#if CustomerName}}");
                if (customerStart >= 0)
                {
                    var customerEnd = html.IndexOf("{{/if}}", customerStart) + 7;
                    html = html.Remove(customerStart, customerEnd - customerStart);
                }
            }

            // Handle notes conditionals
            if (!string.IsNullOrEmpty(data.Notes))
            {
                html = html.Replace("{{#if Notes}}", "").Replace("{{/if}}", "");
                html = html.Replace("{{Notes}}", data.Notes);
            }
            else
            {
                var notesStart = html.IndexOf("{{#if Notes}}");
                if (notesStart >= 0)
                {
                    var notesEnd = html.IndexOf("{{/if}}", notesStart) + 7;
                    html = html.Remove(notesStart, notesEnd - notesStart);
                }
            }

            return html;
        }

        private string GenerateBarcodeBase64(string text)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 50,
                        Margin = 0
                    }
                };

                using var bitmap = writer.Write(text);
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                
                var base64 = Convert.ToBase64String(data.ToArray());
                return $"data:image/png;base64,{base64}";
            }
            catch (Exception ex)
            {
                // Log the error and return a fallback
                Console.WriteLine($"Error generating barcode: {ex.Message}");
                return $"data:text/plain;base64,{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text))}";
            }
        }
    }
}