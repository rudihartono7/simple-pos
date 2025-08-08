using PosSystem.Models;

namespace PosSystem.Services
{
    public interface IStockMovementService
    {
        Task<StockMovement> RecordMovementAsync(StockMovement movement);
        Task<IEnumerable<StockMovement>> GetMovementsByProductAsync(int productId);
        Task<IEnumerable<StockMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockMovement>> GetMovementsByTypeAsync(string movementType);
    }
}