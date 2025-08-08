# SystemSettings Sample Data Documentation

## Overview
The SystemSettingController manages application-wide configuration settings. This document provides sample data to understand how the SystemSetting class is used in a POS system.

## SystemSetting Model Structure
```csharp
public class SystemSetting
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;     // Unique identifier for the setting
    public string SettingValue { get; set; } = string.Empty;  // The actual value (stored as string)
    public string? Description { get; set; }                  // Human-readable description
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last update timestamp
    public int? UpdatedBy { get; set; }                       // User ID who updated
    public User? UpdatedByUser { get; set; }                  // Navigation property
}
```

## Sample System Settings Data

### 1. Store Configuration
```json
{
    "settingKey": "store.name",
    "settingValue": "SuperMart POS",
    "description": "Store display name"
}

{
    "settingKey": "store.address",
    "settingValue": "123 Main Street, Downtown, City 12345",
    "description": "Store physical address"
}

{
    "settingKey": "store.phone",
    "settingValue": "+1-555-0123",
    "description": "Store contact phone number"
}

{
    "settingKey": "store.email",
    "settingValue": "info@supermart.com",
    "description": "Store contact email"
}

{
    "settingKey": "store.tax_id",
    "settingValue": "TAX123456789",
    "description": "Store tax identification number"
}
```

### 2. Tax Configuration
```json
{
    "settingKey": "tax.default_rate",
    "settingValue": "10.5",
    "description": "Default tax rate percentage"
}

{
    "settingKey": "tax.enabled",
    "settingValue": "true",
    "description": "Enable/disable tax calculation"
}

{
    "settingKey": "tax.inclusive",
    "settingValue": "false",
    "description": "Whether prices include tax"
}
```

### 3. Receipt Configuration
```json
{
    "settingKey": "receipt.header_text",
    "settingValue": "Thank you for shopping with us!",
    "description": "Text displayed at the top of receipts"
}

{
    "settingKey": "receipt.footer_text",
    "settingValue": "Please keep your receipt for returns",
    "description": "Text displayed at the bottom of receipts"
}

{
    "settingKey": "receipt.show_barcode",
    "settingValue": "true",
    "description": "Show barcode on receipts"
}

{
    "settingKey": "receipt.auto_print",
    "settingValue": "false",
    "description": "Automatically print receipts after transaction"
}
```

### 4. Payment Configuration
```json
{
    "settingKey": "payment.cash_enabled",
    "settingValue": "true",
    "description": "Enable cash payments"
}

{
    "settingKey": "payment.card_enabled",
    "settingValue": "true",
    "description": "Enable credit/debit card payments"
}

{
    "settingKey": "payment.ewallet_enabled",
    "settingValue": "true",
    "description": "Enable e-wallet payments"
}

{
    "settingKey": "payment.minimum_cash_amount",
    "settingValue": "0.01",
    "description": "Minimum cash payment amount"
}
```

### 5. Inventory Configuration
```json
{
    "settingKey": "inventory.low_stock_threshold",
    "settingValue": "10",
    "description": "Default low stock alert threshold"
}

{
    "settingKey": "inventory.auto_deduct_stock",
    "settingValue": "true",
    "description": "Automatically deduct stock on sales"
}

{
    "settingKey": "inventory.allow_negative_stock",
    "settingValue": "false",
    "description": "Allow selling when stock is negative"
}
```

### 6. Security Configuration
```json
{
    "settingKey": "security.session_timeout",
    "settingValue": "30",
    "description": "Session timeout in minutes"
}

{
    "settingKey": "security.require_pin_for_void",
    "settingValue": "true",
    "description": "Require PIN for voiding transactions"
}

{
    "settingKey": "security.max_login_attempts",
    "settingValue": "3",
    "description": "Maximum failed login attempts before lockout"
}
```

### 7. Display Configuration
```json
{
    "settingKey": "display.currency_symbol",
    "settingValue": "$",
    "description": "Currency symbol to display"
}

{
    "settingKey": "display.currency_code",
    "settingValue": "USD",
    "description": "Currency code (ISO 4217)"
}

{
    "settingKey": "display.decimal_places",
    "settingValue": "2",
    "description": "Number of decimal places for currency"
}

{
    "settingKey": "display.date_format",
    "settingValue": "MM/dd/yyyy",
    "description": "Date display format"
}

{
    "settingKey": "display.time_format",
    "settingValue": "HH:mm:ss",
    "description": "Time display format"
}
```

### 8. Promotion Configuration
```json
{
    "settingKey": "promotion.auto_apply",
    "settingValue": "true",
    "description": "Automatically apply eligible promotions"
}

{
    "settingKey": "promotion.max_discount_percent",
    "settingValue": "50",
    "description": "Maximum discount percentage allowed"
}
```

### 9. Backup Configuration
```json
{
    "settingKey": "backup.auto_backup_enabled",
    "settingValue": "true",
    "description": "Enable automatic database backups"
}

{
    "settingKey": "backup.backup_frequency_hours",
    "settingValue": "24",
    "description": "Backup frequency in hours"
}

{
    "settingKey": "backup.retention_days",
    "settingValue": "30",
    "description": "Number of days to retain backups"
}
```

### 10. Notification Configuration
```json
{
    "settingKey": "notification.email_enabled",
    "settingValue": "true",
    "description": "Enable email notifications"
}

{
    "settingKey": "notification.sms_enabled",
    "settingValue": "false",
    "description": "Enable SMS notifications"
}

{
    "settingKey": "notification.low_stock_alert",
    "settingValue": "true",
    "description": "Send alerts for low stock items"
}
```

## API Usage Examples

### 1. Get All Settings
```http
GET /api/systemsetting
Authorization: Bearer {token}
```

### 2. Get Specific Setting
```http
GET /api/systemsetting/store.name
Authorization: Bearer {token}
```

### 3. Get Setting Value Only
```http
GET /api/systemsetting/tax.default_rate/value
Authorization: Bearer {token}
```

### 4. Create/Update Setting
```http
POST /api/systemsetting
Authorization: Bearer {token}
Content-Type: application/json

{
    "settingKey": "store.name",
    "settingValue": "My New Store",
    "description": "Updated store name"
}
```

### 5. Update Existing Setting
```http
PUT /api/systemsetting/tax.default_rate
Authorization: Bearer {token}
Content-Type: application/json

{
    "settingValue": "12.5",
    "description": "Updated tax rate"
}
```

### 6. Bulk Update Settings
```http
POST /api/systemsetting/bulk
Authorization: Bearer {token}
Content-Type: application/json

{
    "settings": [
        {
            "key": "store.name",
            "value": "SuperMart POS",
            "description": "Store name"
        },
        {
            "key": "tax.default_rate",
            "value": "10.5",
            "description": "Default tax rate"
        }
    ]
}
```

### 7. Get Settings by Category
```http
GET /api/systemsetting/categories/store
Authorization: Bearer {token}
```

### 8. Check if Setting Exists
```http
GET /api/systemsetting/store.name/exists
Authorization: Bearer {token}
```

### 9. Delete Setting
```http
DELETE /api/systemsetting/old.setting.key
Authorization: Bearer {token}
```

## Setting Key Naming Conventions

### Hierarchical Structure
- Use dot notation for hierarchical settings: `category.subcategory.setting`
- Examples:
  - `store.name`
  - `payment.card.enabled`
  - `receipt.printer.default`

### Common Categories
- **store**: Store information and branding
- **tax**: Tax calculation settings
- **payment**: Payment method configurations
- **receipt**: Receipt formatting and printing
- **inventory**: Stock management settings
- **security**: Security and authentication settings
- **display**: UI and formatting preferences
- **promotion**: Discount and promotion rules
- **backup**: Data backup configurations
- **notification**: Alert and notification settings

## Data Types and Validation

### String Values
- Store names, addresses, text content
- Example: `"SuperMart POS"`

### Boolean Values
- Stored as string "true" or "false"
- Example: `"true"`, `"false"`

### Numeric Values
- Stored as string representation
- Example: `"10.5"`, `"30"`, `"100"`

### JSON Objects
- Complex configurations stored as JSON strings
- Example: `"{\"host\":\"localhost\",\"port\":5432}"`

## Best Practices

### 1. Setting Key Design
- Use lowercase with dots for separation
- Be descriptive but concise
- Group related settings with common prefixes

### 2. Default Values
- Always provide sensible defaults
- Document what happens when setting is missing

### 3. Validation
- Validate setting values before saving
- Provide clear error messages for invalid values

### 4. Security
- Don't store sensitive data like passwords in plain text
- Use encryption for sensitive configuration values

### 5. Documentation
- Always provide meaningful descriptions
- Document the impact of changing each setting

This sample data demonstrates how the SystemSettingController manages various aspects of a POS system configuration, from basic store information to complex operational settings.