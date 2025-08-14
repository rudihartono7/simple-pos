# MovementTypes Constants

This document explains the standardized MovementTypes constants used throughout the POS system.

## Overview

The `MovementTypes` class centralizes all stock movement type constants to ensure consistency across the application and eliminate hardcoded strings.

## Constants

### Basic Movement Types
- `IN` - General stock increase
- `OUT` - General stock decrease
- `ADJUSTMENT` - Stock adjustment
- `TRANSFER` - Stock transfer between locations
- `RETURN` - Product return

### Specific Operation Types
- `SALE` - Sale transaction
- `STOCK_IN` - Stock receiving
- `STOCK_OUT` - Stock outgoing
- `TRANSFER_OUT` - Transfer out from warehouse
- `TRANSFER_IN` - Transfer into warehouse
- `ADJUSTMENT_IN` - Positive stock adjustment
- `ADJUSTMENT_OUT` - Negative stock adjustment
- `RESERVE` - Reserve stock
- `UNRESERVE` - Unreserve stock

### Legacy Types (Backward Compatibility)
- `STOCK_IN_LEGACY` - "StockIn"
- `STOCK_OUT_LEGACY` - "StockOut"

## Usage

### In Services
```csharp
using PosSystem.Constants;

// Instead of:
MovementType = "Sale"

// Use:
MovementType = MovementTypes.SALE
```

### In Controllers
```csharp
using PosSystem.Constants;

// Instead of:
var validTypes = new[] { "IN", "OUT", "ADJUSTMENT", "TRANSFER", "RETURN" };

// Use:
if (!MovementTypes.ValidTypes.Contains(movementType.ToUpper()))
```

## Validation Arrays

- `ValidTypes` - Contains the 5 basic movement types for API validation
- `AllTypes` - Contains all movement types including legacy ones

## Files Updated

The following files have been updated to use the standardized constants:

1. `Controllers/StockMovementController.cs`
2. `Services/TransactionService.cs`
3. `Services/StockTransferService.cs`
4. `Services/StockReceivingService.cs`
5. `Services/ProductService.cs`
6. `Services/WarehouseService.cs`

## Benefits

1. **Consistency** - All movement types are defined in one place
2. **Type Safety** - Reduces typos and string literal errors
3. **Maintainability** - Easy to add new movement types or modify existing ones
4. **IntelliSense** - Better IDE support with autocomplete
5. **Refactoring** - Easier to rename or modify movement types

## Migration Notes

Existing data in the database will continue to work as the constants match the existing string values. The system maintains backward compatibility with legacy movement type names.