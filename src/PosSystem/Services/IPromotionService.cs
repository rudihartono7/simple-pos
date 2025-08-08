using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<Promotion?> GetPromotionByCodeAsync(string promotionCode);
        Task<decimal> CalculateDiscountAsync(List<TransactionItem> items, Promotion promotion);
        Task<List<Promotion>> GetApplicablePromotionsAsync(List<TransactionItem> items);
        Task<bool> ApplyPromotionToTransactionAsync(string promotionCode);
        Task<Promotion> CreatePromotionAsync(Promotion promotion);
        Task<Promotion> UpdatePromotionAsync(Promotion promotion);
        Task<bool> DeletePromotionAsync(int id);
        Task<bool> ValidatePromotionAsync(Promotion promotion, List<TransactionItem> items);
    }
}