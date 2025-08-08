using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;
using System.Text.Json;

namespace PosSystem.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;

        public PromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var currentDate = DateTime.UtcNow.Date;
            return await _context.Promotions
                .Where(p => p.IsActive && 
                           p.StartDate <= currentDate && 
                           p.EndDate >= currentDate &&
                           (p.UsageLimit == null || p.UsedCount < p.UsageLimit))
                .OrderBy(p => p.PromotionName)
                .ToListAsync();
        }

        public async Task<Promotion?> GetPromotionByCodeAsync(string promotionCode)
        {
            var currentDate = DateTime.UtcNow.Date;
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionCode == promotionCode && 
                                        p.IsActive && 
                                        p.StartDate <= currentDate && 
                                        p.EndDate >= currentDate &&
                                        (p.UsageLimit == null || p.UsedCount < p.UsageLimit));
        }

        public async Task<decimal> CalculateDiscountAsync(List<TransactionItem> items, Promotion promotion)
        {
            if (!await ValidatePromotionAsync(promotion, items))
                return 0;

            var subtotal = items.Sum(i => i.UnitPrice * i.Quantity);

            switch (promotion.PromotionType.ToLower())
            {
                case "percentage":
                    return subtotal * (promotion.Value / 100);

                case "fixed_amount":
                    return Math.Min(promotion.Value, subtotal);
                case "bogo":
                    return CalculateBogoDiscount(items, promotion);

                case "bundle":
                    return CalculateBundleDiscount(items, promotion);

                case "happyhour":
                    if (IsHappyHour())
                        return subtotal * (promotion.Value / 100);
                    break;

                default:
                    return 0;
            }

            return 0;
        }

        public async Task<List<Promotion>> GetApplicablePromotionsAsync(List<TransactionItem> items)
        {
            var activePromotions = await GetActivePromotionsAsync();
            var applicablePromotions = new List<Promotion>();

            foreach (var promotion in activePromotions)
            {
                if (await ValidatePromotionAsync(promotion, items))
                {
                    applicablePromotions.Add(promotion);
                }
            }

            return applicablePromotions.OrderByDescending(p => CalculateDiscountAsync(items, p).Result).ToList();
        }

        public async Task<Promotion> CreatePromotionAsync(Promotion promotion)
        {
            promotion.CreatedAt = DateTime.UtcNow;
            
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            
            return promotion;
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
            
            return promotion;
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return false;

            promotion.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidatePromotionAsync(Promotion promotion, List<TransactionItem> items)
        {
            // Check if promotion is active and within date range
            var currentDate = DateTime.UtcNow.Date;
            if (!promotion.IsActive || 
                promotion.StartDate > currentDate || 
                promotion.EndDate < currentDate)
                return false;

            // Check usage limit
            if (promotion.UsageLimit.HasValue && promotion.UsedCount >= promotion.UsageLimit.Value)
                return false;

            // Check minimum purchase
            var subtotal = items.Sum(i => i.UnitPrice * i.Quantity);
            if (promotion.MinimumPurchase.HasValue && subtotal < promotion.MinimumPurchase.Value)
                return false;

            // Check applicable products
            if (!string.IsNullOrEmpty(promotion.ApplicableProducts))
            {
                try
                {
                    var applicableProductIds = JsonSerializer.Deserialize<List<int>>(promotion.ApplicableProducts);
                    if (applicableProductIds != null && applicableProductIds.Any())
                    {
                        var hasApplicableProduct = items.Any(i => applicableProductIds.Contains(i.ProductId));
                        if (!hasApplicableProduct)
                            return false;
                    }
                }
                catch (JsonException)
                {
                    // Invalid JSON format, skip product validation
                }
            }

            return true;
        }

        private decimal CalculateBogoDiscount(List<TransactionItem> items, Promotion promotion)
        {
            // Simple BOGO implementation - get cheapest item free for every 2 items
            var totalQuantity = items.Sum(i => (int)i.Quantity);
            var freeItems = totalQuantity / 2;
            
            var sortedItems = items.OrderBy(i => i.UnitPrice).ToList();
            decimal discount = 0;
            
            var remainingFreeItems = freeItems;
            foreach (var item in sortedItems)
            {
                if (remainingFreeItems <= 0) break;
                
                var freeQuantity = Math.Min(remainingFreeItems, (int)item.Quantity);
                discount += item.UnitPrice * freeQuantity;
                remainingFreeItems -= freeQuantity;
            }
            
            return discount;
        }

        private decimal CalculateBundleDiscount(List<TransactionItem> items, Promotion promotion)
        {
            // Simple bundle discount - fixed amount off when buying multiple items
            if (items.Count >= 2)
            {
                return promotion.Value;
            }
            return 0;
        }

        private bool IsHappyHour()
        {
            var currentHour = DateTime.UtcNow.Hour;
            // Happy hour between 2 PM and 5 PM
            return currentHour >= 14 && currentHour < 17;
        }

        public async Task<bool> ApplyPromotionToTransactionAsync(string promotionCode)
        {
            var promotion = await GetPromotionByCodeAsync(promotionCode);
            if (promotion == null) return false;

            promotion.UsedCount++;
            await UpdatePromotionAsync(promotion);
            return true;
        }
    }
}