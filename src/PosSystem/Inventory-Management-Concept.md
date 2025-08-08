# Advanced Inventory Management System - Stock In Concept

## Overview
This document outlines the comprehensive inventory management system that handles stock inflow from multiple sources including suppliers, warehouses, and other locations. The system provides full traceability, cost tracking, and multi-location inventory management.

## Core Concepts

### 1. **Multi-Source Stock Inflow**
The system supports stock receiving from various sources:
- **Suppliers** - Direct purchases from vendors
- **Warehouses** - Inter-location transfers
- **Returns** - Customer/store returns
- **Adjustments** - Manual stock corrections
- **Manufacturing** - Finished goods from production

### 2. **Multi-Location Inventory**
- **Warehouses** - Central storage facilities
- **Stores** - Retail locations with POS systems
- **Transit** - Goods in movement between locations
- **Virtual** - Consignment or drop-ship inventory

## Data Models Architecture

### **Supplier Management**
```csharp
public class Supplier
{
    public int Id { get; set; }
    public string SupplierName { get; set; }
    public string? SupplierCode { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "Indonesia";
    public string? TaxId { get; set; }
    public decimal CreditLimit { get; set; } = 0;
    public int PaymentTermDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    public ICollection<StockReceiving> StockReceivings { get; set; }
}
```

### **Purchase Order System**
```csharp
public class PurchaseOrder
{
    public int Id { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string Status { get; set; } = "DRAFT"; // DRAFT, PENDING, APPROVED, ORDERED, RECEIVED, CANCELLED
    public decimal SubTotal { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<PurchaseOrderItem> Items { get; set; }
    public ICollection<StockReceiving> StockReceivings { get; set; }
}

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityReceived { get; set; } = 0;
    // Calculated property: QuantityPending = QuantityOrdered - QuantityReceived
    public decimal UnitCost { get; set; }
    public decimal TotalCost => QuantityOrdered * UnitCost;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### **Stock Receiving System**
```csharp
public class StockReceiving
{
    public int Id { get; set; }
    public string ReceivingNumber { get; set; }
    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public DateTime ReceivingDate { get; set; } = DateTime.UtcNow;
    public string? SupplierInvoiceNumber { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string Status { get; set; } = "DRAFT"; // DRAFT, RECEIVED, COMPLETED, CANCELLED
    public decimal SubTotal { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal TotalCost { get; set; } = 0;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<StockReceivingItem> Items { get; set; }
}

public class StockReceivingItem
{
    public int Id { get; set; }
    public int StockReceivingId { get; set; }
    public StockReceiving StockReceiving { get; set; }
    public int? PurchaseOrderItemId { get; set; }
    public PurchaseOrderItem? PurchaseOrderItem { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public decimal QuantityOrdered { get; set; } = 0;
    public decimal QuantityReceived { get; set; }
    public decimal QuantityAccepted { get; set; }
    public decimal QuantityRejected { get; set; } = 0;
    public decimal UnitCost { get; set; }
    public decimal TotalCost => QuantityAccepted * UnitCost;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string QualityStatus { get; set; } = "GOOD"; // GOOD, DAMAGED, EXPIRED, REJECTED
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### **Warehouse Management**
```csharp
public class Warehouse
{
    public int Id { get; set; }
    public string WarehouseName { get; set; }
    public string? WarehouseCode { get; set; }
    public string WarehouseType { get; set; } = "MAIN"; // MAIN, BRANCH, TRANSIT, VIRTUAL
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "Indonesia";
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<WarehouseStock> Stocks { get; set; }
    public ICollection<StockTransfer> TransfersFrom { get; set; }
    public ICollection<StockTransfer> TransfersTo { get; set; }
    public ICollection<StockReceiving> StockReceivings { get; set; }
}
```

### **Multi-Location Stock Tracking**
```csharp
public class WarehouseStock
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public decimal QuantityOnHand { get; set; } = 0;
    public decimal QuantityReserved { get; set; } = 0;
    // Calculated property: QuantityAvailable = QuantityOnHand - QuantityReserved
    public decimal MinStockLevel { get; set; } = 0;
    public decimal MaxStockLevel { get; set; } = 0;
    public decimal ReorderPoint { get; set; } = 0;
    public decimal AverageCost { get; set; } = 0;
    public decimal LastCost { get; set; } = 0;
    public string? Location { get; set; } // Aisle-Shelf-Bin format
    public DateTime? LastMovementDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### **Stock Transfer System**
```csharp
public class StockTransfer
{
    public int Id { get; set; }
    public string TransferNumber { get; set; }
    public int FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; }
    public int ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; }
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string Status { get; set; } = "DRAFT"; // DRAFT, APPROVED, SHIPPED, RECEIVED, CANCELLED
    public string TransferType { get; set; } = "REGULAR"; // REGULAR, EMERGENCY, RETURN
    public string? Reason { get; set; }
    public decimal TotalValue { get; set; } = 0;
    public string? Notes { get; set; }
    public string? ShippingMethod { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<StockTransferItem> Items { get; set; }
}

public class StockTransferItem
{
    public int Id { get; set; }
    public int StockTransferId { get; set; }
    public StockTransfer StockTransfer { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int FromWarehouseStockId { get; set; }
    public WarehouseStock FromWarehouseStock { get; set; }
    public int ToWarehouseStockId { get; set; }
    public WarehouseStock ToWarehouseStock { get; set; }
    public decimal QuantityRequested { get; set; }
    public decimal QuantityShipped { get; set; } = 0;
    public decimal QuantityReceived { get; set; } = 0;
    public decimal UnitCost { get; set; }
    public decimal TotalValue => QuantityReceived * UnitCost;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

## Stock In Workflows

### 1. **Purchase Order to Stock Receiving Flow**

#### Step 1: Create Purchase Order
```
1. Select Supplier
2. Add Products with quantities and costs
3. Set delivery expectations
4. Submit for approval
5. Send to supplier
```

#### Step 2: Receive Goods
```
1. Create Stock Receiving document
2. Reference Purchase Order
3. Record actual quantities received
4. Perform quality inspection
5. Accept/Reject items
6. Update stock levels
```

#### Step 3: Post to Inventory
```
1. Validate receiving document
2. Update product costs (FIFO/LIFO/Average)
3. Create stock movement records
4. Update warehouse stock levels
5. Generate accounting entries
```

### 2. **Inter-Warehouse Transfer Flow**

#### Step 1: Create Transfer Request
```
1. Select source warehouse
2. Select destination warehouse
3. Add products and quantities
4. Set transfer reason/type
5. Submit for approval
```

#### Step 2: Ship from Source
```
1. Pick items from source warehouse
2. Update source stock (reserved → shipped)
3. Create shipping document
4. Track shipment status
```

#### Step 3: Receive at Destination
```
1. Receive shipment
2. Verify quantities and condition
3. Update destination stock
4. Complete transfer process
5. Update stock movement records
```

### 3. **Direct Stock In (Manual Entry)**

#### For Emergency/Adjustment Situations
```
1. Select warehouse/location
2. Select product
3. Enter quantity and reason
4. Set cost information
5. Add batch/expiry details
6. Create stock movement record
```

## Key Features

### **1. Batch and Lot Tracking**
- Track products by batch numbers
- Monitor expiry dates for perishable items
- FIFO/FEFO rotation management
- Full traceability for product recalls
- Quality status tracking per batch

### **2. Cost Management**
- Weighted average costing method (primary implementation)
- Last cost tracking for recent purchase prices
- Landed cost calculations including taxes and fees
- Cost variance analysis and reporting
- Multi-currency support for international suppliers

### **3. Quality Control**
- Inspection workflows during receiving
- Accept/reject processes with reason codes
- Quality status tracking (GOOD, DAMAGED, EXPIRED, REJECTED)
- Damage/defect recording and reporting
- Supplier quality performance metrics

### **4. Approval Workflows**
- Purchase order approval hierarchy
- Transfer authorization based on value thresholds
- Receiving confirmation requirements
- Stock adjustment approvals
- Role-based access control

### **5. Real-time Inventory**
- Live stock levels across all locations
- Reserved quantity tracking for pending orders
- Available-to-promise calculations
- Low stock alerts with configurable thresholds
- Stock movement audit trail

### **6. Multi-Location Support**
- Warehouse hierarchy (MAIN, BRANCH, TRANSIT, VIRTUAL)
- Inter-location stock transfers
- Location-specific stock levels and parameters
- Centralized inventory visibility
- Location-based picking and fulfillment

## API Endpoints Structure

### **Supplier Management**
```
GET    /api/supplier                    - List all suppliers
POST   /api/supplier                    - Create supplier
GET    /api/supplier/{id}               - Get supplier details
PUT    /api/supplier/{id}               - Update supplier
DELETE /api/supplier/{id}               - Deactivate supplier
```

### **Purchase Orders**
```
GET    /api/purchase-order                    - List purchase orders
POST   /api/purchase-order                    - Create purchase order
GET    /api/purchase-order/{id}               - Get PO details
PUT    /api/purchase-order/{id}               - Update PO
DELETE /api/purchase-order/{id}               - Delete PO
POST   /api/purchase-order/{id}/approve       - Approve PO
POST   /api/purchase-order/{id}/cancel        - Cancel PO
GET    /api/purchase-order/generate-number    - Generate PO number
POST   /api/purchase-order/{id}/calculate     - Calculate PO totals
```

### **Stock Receiving**
```
GET    /api/stock-receiving                   - List receiving documents
POST   /api/stock-receiving                   - Create receiving
GET    /api/stock-receiving/{id}              - Get receiving details
PUT    /api/stock-receiving/{id}              - Update receiving
DELETE /api/stock-receiving/{id}              - Delete receiving
POST   /api/stock-receiving/{id}/complete     - Complete receiving
POST   /api/stock-receiving/{id}/cancel       - Cancel receiving
GET    /api/stock-receiving/generate-number   - Generate receiving number
POST   /api/stock-receiving/{id}/process      - Process receiving
POST   /api/stock-receiving/{id}/calculate    - Calculate receiving totals
```

### **Stock Transfer**
```
GET    /api/stock-transfer                    - List stock transfers
POST   /api/stock-transfer                    - Create stock transfer
GET    /api/stock-transfer/{id}               - Get transfer details
PUT    /api/stock-transfer/{id}               - Update transfer
DELETE /api/stock-transfer/{id}               - Delete transfer
POST   /api/stock-transfer/{id}/approve       - Approve transfer
POST   /api/stock-transfer/{id}/ship          - Ship transfer
POST   /api/stock-transfer/{id}/receive       - Receive transfer
POST   /api/stock-transfer/{id}/cancel        - Cancel transfer
GET    /api/stock-transfer/generate-number    - Generate transfer number
```

### **Warehouse Management**
```
GET    /api/warehouse                         - List warehouses
POST   /api/warehouse                         - Create warehouse
GET    /api/warehouse/{id}                    - Get warehouse details
PUT    /api/warehouse/{id}                    - Update warehouse
DELETE /api/warehouse/{id}                    - Delete warehouse
GET    /api/warehouse/{id}/stock              - Get warehouse stock
POST   /api/warehouse/{id}/stock              - Update warehouse stock
GET    /api/warehouse/{id}/movements          - Get stock movements
POST   /api/warehouse/stock/movement          - Record stock movement
```

### **Inventory Queries**
```
GET    /api/warehouse/stock/levels            - Current stock by location
GET    /api/warehouse/stock/low-stock         - Low stock alerts
GET    /api/warehouse/stock/valuation         - Inventory valuation
GET    /api/warehouse/stock/movements         - Stock movement history
GET    /api/warehouse/stock/aging             - Stock aging report
GET    /api/warehouse/stock/availability      - Stock availability check
```

## Business Rules

### **1. Stock Receiving Rules**
- Cannot receive more than ordered quantity (with configurable tolerance)
- Must validate supplier and PO before receiving
- Quality inspection required for certain product categories
- Automatic cost updates based on costing method (Average Cost)
- Support for partial receiving and over-receiving scenarios
- Batch/lot tracking for products with expiry dates
- Quality status tracking (GOOD, DAMAGED, EXPIRED, REJECTED)

### **2. Transfer Rules**
- Cannot transfer more than available quantity
- Source and destination warehouses must be different
- Approval workflow for high-value transfers
- Automatic reservation during transfer process
- Support for different transfer types (REGULAR, EMERGENCY, RETURN)
- Tracking of shipped vs received quantities
- Batch/lot preservation during transfers

### **3. Cost Calculation Rules**
- **Average Cost**: Weighted average costing method (primary)
- **Last Cost**: Most recent purchase cost tracking
- **Standard Cost**: Predetermined standard costs (if applicable)
- Automatic cost updates on stock receiving
- Cost variance tracking and reporting

### **4. Location Rules**
- Each product can exist in multiple locations
- Location-specific min/max/reorder levels
- Automatic reorder point calculations
- Location-based picking priorities
- Support for virtual warehouses (consignment, drop-ship)
- Aisle-Shelf-Bin location tracking

## Integration Points

### **1. POS System Integration**
- Real-time stock deduction on sales
- Multi-location stock checking
- Automatic stock reservation
- Cross-location fulfillment

### **2. Accounting Integration**
- Inventory valuation updates
- Cost of goods sold calculations
- Purchase accruals
- Inventory adjustments

### **3. Supplier Integration**
- EDI purchase orders
- Electronic invoicing
- Delivery notifications
- Catalog synchronization

## Reporting Capabilities

### **1. Operational Reports**
- Stock levels by location
- Pending purchase orders
- Receiving schedules
- Transfer status reports

### **2. Financial Reports**
- Inventory valuation
- Cost variance analysis
- Purchase spend analysis
- Carrying cost reports

### **3. Performance Reports**
- Inventory turnover
- Stock accuracy
- Supplier performance
- Fill rate analysis

## Implementation Benefits

### **1. Operational Benefits**
- Centralized inventory visibility across all locations
- Automated stock replenishment with configurable reorder points
- Reduced stockouts through real-time availability tracking
- Improved supplier relationships with structured PO processes
- Enhanced quality control with inspection workflows
- Streamlined inter-location transfers

### **2. Financial Benefits**
- Accurate inventory valuation using weighted average costing
- Better cash flow management through purchase order controls
- Reduced carrying costs with optimized stock levels
- Improved gross margins through cost tracking and analysis
- Tax compliance with proper documentation
- Supplier payment term management

### **3. Compliance Benefits**
- Complete audit trail for all inventory movements
- Batch/lot traceability for product recalls and quality issues
- Regulatory compliance for food and pharmaceutical products
- Quality assurance documentation and reporting
- Financial audit support with detailed cost tracking
- Role-based access control for security compliance

### **4. Technical Benefits**
- RESTful API design for easy integration
- Real-time data synchronization across modules
- Scalable multi-location architecture
- Comprehensive reporting and analytics
- Mobile-friendly responsive design
- Extensible framework for future enhancements

This comprehensive inventory management system provides complete control over stock inflow from multiple sources while maintaining accuracy, traceability, and cost control across all locations.