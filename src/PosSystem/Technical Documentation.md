# Technical Documentation - POS System
## ASP.NET Core 9 MVC with PostgreSQL

**Version:** 1.0  
**Date:** 2025-08-06  
**Technology Stack:** ASP.NET Core 9 MVC, PostgreSQL, TailwindCSS  

---

## 1. Database Structure

### 1.1 Entity Relationship Diagram Overview

```sql
-- Core Tables
Users (Authentication & Authorization)
Stores (Multi-branch support)
Products (Inventory items)
Categories (Product categorization)
Customers (Customer management)
Transactions (Sales records)
TransactionItems (Line items)
Payments (Payment records)
Promotions (Discount rules)
StockMovements (Inventory tracking)
```

### 1.2 Database Schema

```sql
-- Users Table
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Manager', 'Cashier', 'Inventory', 'Auditor')),
    PIN VARCHAR(10),
    StoreId INT,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id)
);

-- Stores Table
CREATE TABLE Stores (
    Id SERIAL PRIMARY KEY,
    StoreName VARCHAR(100) NOT NULL,
    StoreCode VARCHAR(20) UNIQUE NOT NULL,
    Address TEXT,
    Phone VARCHAR(20),
    TaxRate DECIMAL(5,4) DEFAULT 0.11,
    Currency VARCHAR(3) DEFAULT 'IDR',
    Timezone VARCHAR(50) DEFAULT 'Asia/Jakarta',
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Categories Table
CREATE TABLE Categories (
    Id SERIAL PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL,
    Description TEXT,
    ParentCategoryId INT,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id)
);

-- Products Table
CREATE TABLE Products (
    Id SERIAL PRIMARY KEY,
    ProductCode VARCHAR(50) UNIQUE NOT NULL,
    Barcode VARCHAR(100) UNIQUE,
    ProductName VARCHAR(200) NOT NULL,
    Description TEXT,
    CategoryId INT NOT NULL,
    UnitPrice DECIMAL(15,2) NOT NULL,
    CostPrice DECIMAL(15,2),
    StockQuantity INT DEFAULT 0,
    MinStockLevel INT DEFAULT 0,
    UnitOfMeasure VARCHAR(20) DEFAULT 'PCS',
    HasVariants BOOLEAN DEFAULT false,
    IsActive BOOLEAN DEFAULT true,
    HasExpiry BOOLEAN DEFAULT false,
    RequiresWeighing BOOLEAN DEFAULT false,
    ImageUrl VARCHAR(500),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Product Variants Table
CREATE TABLE ProductVariants (
    Id SERIAL PRIMARY KEY,
    ProductId INT NOT NULL,
    VariantName VARCHAR(100) NOT NULL,
    VariantValue VARCHAR(100) NOT NULL,
    UnitPrice DECIMAL(15,2),
    StockQuantity INT DEFAULT 0,
    Barcode VARCHAR(100) UNIQUE,
    IsActive BOOLEAN DEFAULT true,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Customers Table
CREATE TABLE Customers (
    Id SERIAL PRIMARY KEY,
    CustomerCode VARCHAR(20) UNIQUE,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    Phone VARCHAR(20),
    DateOfBirth DATE,
    Address TEXT,
    CustomerGroup VARCHAR(20) DEFAULT 'Regular',
    LoyaltyPoints INT DEFAULT 0,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Transactions Table
CREATE TABLE Transactions (
    Id SERIAL PRIMARY KEY,
    TransactionNumber VARCHAR(50) UNIQUE NOT NULL,
    StoreId INT NOT NULL,
    UserId INT NOT NULL,
    CustomerId INT,
    TransactionDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    SubTotal DECIMAL(15,2) NOT NULL,
    DiscountAmount DECIMAL(15,2) DEFAULT 0,
    TaxAmount DECIMAL(15,2) DEFAULT 0,
    TotalAmount DECIMAL(15,2) NOT NULL,
    Status VARCHAR(20) DEFAULT 'Completed' CHECK (Status IN ('Pending', 'Completed', 'Refunded', 'Cancelled', 'Hold')),
    PaymentStatus VARCHAR(20) DEFAULT 'Paid' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Partial', 'Refunded')),
    Notes TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

-- Transaction Items Table
CREATE TABLE TransactionItems (
    Id SERIAL PRIMARY KEY,
    TransactionId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductVariantId INT,
    Quantity DECIMAL(10,3) NOT NULL,
    UnitPrice DECIMAL(15,2) NOT NULL,
    DiscountAmount DECIMAL(15,2) DEFAULT 0,
    LineTotal DECIMAL(15,2) NOT NULL,
    FOREIGN KEY (TransactionId) REFERENCES Transactions(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id)
);

-- Payments Table
CREATE TABLE Payments (
    Id SERIAL PRIMARY KEY,
    TransactionId INT NOT NULL,
    PaymentMethod VARCHAR(20) NOT NULL CHECK (PaymentMethod IN ('Cash', 'CreditCard', 'DebitCard', 'eWallet', 'QRIS', 'BankTransfer')),
    Amount DECIMAL(15,2) NOT NULL,
    ReceivedAmount DECIMAL(15,2),
    ChangeAmount DECIMAL(15,2) DEFAULT 0,
    ReferenceNumber VARCHAR(100),
    PaymentDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) DEFAULT 'Success' CHECK (Status IN ('Pending', 'Success', 'Failed', 'Cancelled')),
    FOREIGN KEY (TransactionId) REFERENCES Transactions(Id)
);

-- Promotions Table
CREATE TABLE Promotions (
    Id SERIAL PRIMARY KEY,
    PromotionName VARCHAR(100) NOT NULL,
    PromotionCode VARCHAR(50) UNIQUE,
    PromotionType VARCHAR(20) NOT NULL CHECK (PromotionType IN ('Percentage', 'FixedAmount', 'BOGO', 'Bundle', 'HappyHour')),
    Value DECIMAL(15,2) NOT NULL,
    MinimumPurchase DECIMAL(15,2),
    ApplicableProducts TEXT, -- JSON array of product IDs
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsActive BOOLEAN DEFAULT true,
    UsageLimit INT,
    UsedCount INT DEFAULT 0,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stock Movements Table
CREATE TABLE StockMovements (
    Id SERIAL PRIMARY KEY,
    ProductId INT NOT NULL,
    ProductVariantId INT,
    MovementType VARCHAR(20) NOT NULL CHECK (MovementType IN ('StockIn', 'StockOut', 'Sale', 'Return', 'Adjustment', 'Transfer')),
    Quantity DECIMAL(10,3) NOT NULL,
    UnitCost DECIMAL(15,2),
    ReferenceId INT, -- Transaction ID or Stock Adjustment ID
    ReferenceType VARCHAR(20), -- 'Transaction', 'Adjustment', 'Transfer'
    Notes TEXT,
    BatchNumber VARCHAR(50),
    ExpiryDate DATE,
    UserId INT NOT NULL,
    MovementDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Audit Logs Table
CREATE TABLE AuditLogs (
    Id SERIAL PRIMARY KEY,
    UserId INT NOT NULL,
    Action VARCHAR(100) NOT NULL,
    TableName VARCHAR(50),
    RecordId INT,
    OldValues JSONB,
    NewValues JSONB,
    IPAddress INET,
    UserAgent TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- System Settings Table
CREATE TABLE SystemSettings (
    Id SERIAL PRIMARY KEY,
    SettingKey VARCHAR(100) UNIQUE NOT NULL,
    SettingValue TEXT NOT NULL,
    Description TEXT,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy INT,
    FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);
```

### 1.3 Indexes for Performance

```sql
-- Performance Indexes
CREATE INDEX idx_transactions_date ON Transactions(TransactionDate);
CREATE INDEX idx_transactions_store ON Transactions(StoreId);
CREATE INDEX idx_transactions_user ON Transactions(UserId);
CREATE INDEX idx_products_barcode ON Products(Barcode);
CREATE INDEX idx_products_category ON Products(CategoryId);
CREATE INDEX idx_stock_movements_product ON StockMovements(ProductId);
CREATE INDEX idx_stock_movements_date ON StockMovements(MovementDate);
CREATE INDEX idx_audit_logs_user_date ON AuditLogs(UserId, CreatedAt);
```

---

## 2. Features and Business Flow

### 2.1 Sales Management Module

#### 2.1.1 Create New Sale Transaction

**Business Flow:**
1. Cashier logs in with PIN/Username
2. System creates new transaction record with "Pending" status
3. Cashier scans barcode or manually searches product
4. System validates product availability and adds to cart
5. Apply discounts/promotions if applicable
6. Calculate taxes and totals
7. Process payment(s)
8. Generate receipt and update inventory
9. Complete transaction

**Technical Implementation:**
```csharp
// Controllers/SalesController.cs
[Authorize(Roles = "Cashier,Manager,Admin")]
public class SalesController : Controller
{
    public async Task<IActionResult> CreateSale()
    {
        var model = new SaleViewModel
        {
            TransactionNumber = GenerateTransactionNumber(),
            TransactionDate = DateTime.UtcNow
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProduct(string barcode, int quantity = 1)
    {
        // Business logic for adding product to cart
    }
    
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PaymentRequest request)
    {
        // Payment processing logic
    }
}
```

#### 2.1.2 Hold/Resume Transaction

**Business Flow:**
1. Cashier clicks "Hold" during active transaction
2. System saves transaction with "Hold" status
3. To resume: Search held transactions, select and continue
4. System loads previous cart state

#### 2.1.3 Refund & Return

**Business Flow:**
1. Manager/Admin searches original transaction
2. Select items to refund
3. Choose refund method (cash, credit to customer account)
4. Create negative transaction record
5. Update inventory (add back to stock)
6. Print refund receipt

### 2.2 Inventory Management Module

#### 2.2.1 Product Management

**Business Flow:**
1. Add/Edit product information
2. Generate/assign barcode
3. Set pricing and stock levels
4. Configure variants if applicable
5. Set minimum stock alerts

**Technical Implementation:**
```csharp
// Controllers/ProductsController.cs
[Authorize(Roles = "Inventory,Manager,Admin")]
public class ProductsController : Controller
{
    public async Task<IActionResult> Index(ProductFilterModel filter)
    {
        // List products with filtering and pagination
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateModel model)
    {
        // Create new product with validation
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateStock(int productId, int quantity, string movementType)
    {
        // Update stock levels with movement tracking
    }
}
```

#### 2.2.2 Stock Movement Tracking

**Business Flow:**
1. Record all stock changes (in/out/adjustments)
2. Track batch numbers and expiry dates
3. Generate movement reports
4. Automatic stock deduction on sales

### 2.3 Customer Management Module

#### 2.3.1 Customer Registration

**Business Flow:**
1. Collect customer information during checkout
2. Assign customer code
3. Set customer group (Regular/VIP)
4. Initialize loyalty points if applicable

#### 2.3.2 Loyalty Points System

**Business Flow:**
1. Calculate points on purchase (configurable rate)
2. Allow points redemption
3. Track points history
4. Send birthday promotions

### 2.4 Promotions & Discounts Module

#### 2.4.1 Automatic Promotion Application

**Business Flow:**
1. System checks applicable promotions during checkout
2. Applies best discount automatically
3. Handles BOGO, percentage, and bundle discounts
4. Validates promotion conditions

**Technical Implementation:**
```csharp
// Services/PromotionService.cs
public class PromotionService
{
    public async Task<List<AppliedPromotion>> CalculatePromotions(Cart cart)
    {
        // Logic to calculate and apply best promotions
    }
    
    public async Task<decimal> ApplyDiscount(string promoCode, Cart cart)
    {
        // Manual promo code application
    }
}
```

### 2.5 Reporting & Analytics Module

#### 2.5.1 Sales Reports

**Available Reports:**
- Daily Sales Summary
- Sales by Product/Category
- Cashier Performance
- Top Selling Products
- Profit Margin Analysis
- Stock Movement Reports

**Technical Implementation:**
```csharp
// Controllers/ReportsController.cs
[Authorize(Roles = "Manager,Admin,Auditor")]
public class ReportsController : Controller
{
    public async Task<IActionResult> DailySales(DateTime date)
    {
        // Generate daily sales report
    }
    
    public async Task<IActionResult> ExportReport(string reportType, ReportFilter filter)
    {
        // Export to Excel/PDF
    }
}
```

---

## 3. Acceptance Criteria for Testing

### 3.1 Sales Management Testing

#### 3.1.1 Create Sale Transaction
**Scenario:** Successful product sale
```
Given: Cashier is logged in
When: Cashier scans product barcode "123456789"
And: Product exists with sufficient stock
And: Processes cash payment of 50000 IDR for 45000 IDR total
Then: Transaction is completed successfully
And: Stock is reduced by 1
And: Receipt is generated
And: Change of 5000 IDR is calculated
And: Transaction record is saved with "Completed" status
```

**Scenario:** Insufficient stock
```
Given: Product has 0 stock quantity
When: Cashier attempts to add product to cart
Then: System shows "Insufficient stock" error
And: Product is not added to cart
```

#### 3.1.2 Multi-Payment Processing
**Scenario:** Split payment (Cash + Card)
```
Given: Transaction total is 100000 IDR
When: Customer pays 50000 IDR cash and 50000 IDR by card
Then: Both payments are recorded separately
And: Total payment equals transaction amount
And: Transaction status is "Completed"
```

### 3.2 Inventory Management Testing

#### 3.2.1 Stock Movement Tracking
**Scenario:** Automatic stock deduction on sale
```
Given: Product has stock quantity of 10
When: 2 units are sold in a transaction
Then: Stock quantity becomes 8
And: Stock movement record is created with type "Sale"
And: Movement references the transaction ID
```

#### 3.2.2 Low Stock Alert
**Scenario:** Stock below minimum level
```
Given: Product minimum stock level is 5
When: Stock quantity reaches 3 after a sale
Then: System generates low stock alert
And: Alert is visible in dashboard
And: Notification is sent to inventory manager
```

### 3.3 User Authentication & Authorization Testing

#### 3.3.1 Role-based Access Control
**Scenario:** Cashier accessing reports
```
Given: User is logged in with "Cashier" role
When: User attempts to access reports page
Then: Access is denied with "Unauthorized" message
And: User is redirected to appropriate page
```

#### 3.3.2 PIN Authentication
**Scenario:** Cashier login with PIN
```
Given: Cashier enters correct 4-digit PIN
When: PIN is validated
Then: User is successfully authenticated
And: Session is created
And: User redirects to POS interface
```

### 3.4 Promotions & Discounts Testing

#### 3.4.1 BOGO Promotion
**Scenario:** Buy One Get One Free
```
Given: BOGO promotion is active for Product A
When: Customer adds 2 units of Product A to cart
Then: Only 1 unit is charged
And: Discount amount equals price of 1 unit
And: Promotion is recorded in transaction
```

#### 3.4.2 Percentage Discount
**Scenario:** 10% store-wide discount
```
Given: 10% discount promotion is active
When: Cart subtotal is 100000 IDR
Then: Discount amount is 10000 IDR
And: Final total is 90000 IDR + tax
```

### 3.5 Data Integrity & Security Testing

#### 3.5.1 Transaction Integrity
**Scenario:** Concurrent transaction processing
```
Given: Two cashiers attempt to sell last unit of product simultaneously
When: Both transactions are processed
Then: Only first transaction succeeds
And: Second transaction shows "Insufficient stock" error
And: Stock quantity remains consistent
```

#### 3.5.2 Audit Trail
**Scenario:** Transaction modification tracking
```
Given: Manager voids a completed transaction
When: Void operation is performed
Then: Audit log records the action
And: Original transaction data is preserved
And: Void reason is mandatory and recorded
```

### 3.6 Hardware Integration Testing

#### 3.6.1 Barcode Scanner Integration
**Scenario:** Successful barcode scan
```
Given: Barcode scanner is connected
When: Valid barcode is scanned
Then: Product is automatically added to cart
And: Product details are displayed
And: Focus remains on scanner input
```

#### 3.6.2 Receipt Printer Integration
**Scenario:** Print customer receipt
```
Given: Transaction is completed
When: Print receipt is triggered
Then: Receipt prints with all transaction details
And: Receipt includes store information
And: Receipt shows payment method and change
```

### 3.7 Performance Testing

#### 3.7.1 Transaction Processing Speed
**Scenario:** Transaction completion time
```
Given: Standard sale transaction with 5 items
When: Transaction is processed
Then: Total processing time is under 2 seconds
And: Receipt is generated within 1 second
And: Database updates are completed successfully
```

### 3.8 Offline Mode Testing

#### 3.8.1 Internet Connection Loss
**Scenario:** Continue sales during offline mode
```
Given: System is operating normally
When: Internet connection is lost
Then: POS continues to function for sales
And: Transactions are stored locally
And: Sync occurs when connection is restored
And: No data is lost during offline period
```

### 3.9 Multi-Store Testing

#### 3.9.1 Store-Specific Data Isolation
**Scenario:** Cross-store data access
```
Given: User belongs to Store A
When: User attempts to access Store B data
Then: Access is denied
And: Only Store A data is visible
And: Store B transactions are not accessible
```

---

## 4. UI/UX Design Guidelines

### 4.1 Admin Panel Layout
- **Sidebar Navigation**: Fixed left sidebar with collapsible menu
- **Main Content Area**: Responsive content with breadcrumbs
- **Header**: User info, notifications, search bar
- **Dashboard**: Cards with key metrics and charts

### 4.2 POS Interface
- **Clean Layout**: Minimal distractions for cashiers
- **Large Touch Targets**: Suitable for touch screens
- **Quick Access**: Frequently used functions prominently displayed
- **Real-time Updates**: Live cart updates and calculations

### 4.3 TailwindCSS Implementation
```html
<!-- Example Dashboard Card -->
<div class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
    <div class="flex items-center justify-between">
        <div>
            <h3 class="text-lg font-semibold text-gray-800">Today's Sales</h3>
            <p class="text-3xl font-bold text-green-600">Rp 2,450,000</p>
        </div>
        <div class="p-3 bg-green-100 rounded-full">
            <svg class="w-8 h-8 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                <!-- Icon SVG -->
            </svg>
        </div>
    </div>
</div>
```

This technical documentation provides a comprehensive foundation for developing your POS system. Each section includes detailed specifications that your development team can follow to ensure all business requirements are met while maintaining code quality and system reliability.