namespace PosSystem.Constants
{
    /// <summary>
    /// Constants for transaction status values used throughout the application
    /// </summary>
    public static class TransactionStatus
    {
        // Transaction status types
        public const string PENDING = "Pending";
        public const string COMPLETED = "Completed";
        public const string HOLD = "Hold";
        public const string CANCELLED = "Cancelled";
        public const string REFUNDED = "Refunded";
        
        /// <summary>
        /// Array of all valid transaction status types for validation
        /// </summary>
        public static readonly string[] ValidStatuses = 
        {
            PENDING, COMPLETED, HOLD, CANCELLED, REFUNDED
        };
    }
}