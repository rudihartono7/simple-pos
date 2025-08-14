namespace PosSystem.Constants
{
    /// <summary>
    /// Constants for stock movement types used throughout the application
    /// </summary>
    public static class MovementTypes
    {
        // Basic movement types
        public const string IN = "IN";
        public const string OUT = "OUT";
        public const string ADJUSTMENT = "ADJUSTMENT";
        public const string TRANSFER = "TRANSFER";
        public const string RETURN = "RETURN";
        
        // Specific operation types
        public const string SALE = "Sale";
        public const string STOCK_IN = "STOCK_IN";
        public const string STOCK_OUT = "STOCK_OUT";
        public const string TRANSFER_OUT = "TRANSFER_OUT";
        public const string TRANSFER_IN = "TRANSFER_IN";
        public const string ADJUSTMENT_IN = "ADJUSTMENT_IN";
        public const string ADJUSTMENT_OUT = "ADJUSTMENT_OUT";
        public const string RESERVE = "RESERVE";
        public const string UNRESERVE = "UNRESERVE";
        
        // Legacy types (for backward compatibility)
        public const string STOCK_IN_LEGACY = "StockIn";
        public const string STOCK_OUT_LEGACY = "StockOut";
        
        /// <summary>
        /// Array of all valid movement types for validation
        /// </summary>
        public static readonly string[] ValidTypes = 
        {
            IN, OUT, ADJUSTMENT, TRANSFER, RETURN
        };
        
        /// <summary>
        /// Array of all movement types including legacy ones
        /// </summary>
        public static readonly string[] AllTypes = 
        {
            IN, OUT, ADJUSTMENT, TRANSFER, RETURN,
            SALE, STOCK_IN, STOCK_OUT, TRANSFER_OUT, TRANSFER_IN,
            ADJUSTMENT_IN, ADJUSTMENT_OUT, RESERVE, UNRESERVE,
            STOCK_IN_LEGACY, STOCK_OUT_LEGACY
        };
    }
}