# POS System API Documentation for Frontend Development

## Table of Contents
1. [Overview](#overview)
2. [Authentication & Authorization](#authentication--authorization)
3. [Core Business Processes](#core-business-processes)
4. [API Endpoints Reference](#api-endpoints-reference)
5. [Data Models](#data-models)
6. [Error Handling](#error-handling)
7. [Best Practices](#best-practices)

## Overview

This POS (Point of Sale) system provides a comprehensive REST API for managing retail operations including sales transactions, inventory management, customer management, and reporting. The system is built with ASP.NET Core and follows RESTful principles.

**Base URL**: `https://your-domain.com/api`

**Content-Type**: `application/json` (except for file uploads which use `multipart/form-data`)

## Authentication & Authorization

### Authentication Flow

The system uses JWT (JSON Web Token) based authentication with refresh tokens.

#### 1. Login Process
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "user@example.com",
    "role": "Cashier",
    "storeId": 1
  }
}
```

#### 2. Token Refresh
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "refresh_token_here"
}
```

#### 3. Authorization Header
For all protected endpoints, include the JWT token in the Authorization header:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Core Business Processes

### 1. Sales Transaction Flow

The sales process follows this sequence:

1. **Create Transaction** → 2. **Add Items** → 3. **Apply Discounts/Promotions** → 4. **Process Payment** → 5. **Complete Transaction**

#### Step-by-Step Implementation:

**Step 1: Create Transaction**
```http
POST /api/transaction
{
  "storeId": 1,
  "customerId": 123  // Optional
}
```

**Step 2: Add Items to Transaction**
```http
POST /api/transaction/{transactionId}/items
{
  "productId": 456,
  "quantity": 2,
  "unitPrice": 25.00,
  "discountAmount": 0
}
```

**Step 3: Process Payment**
```http
POST /api/transaction/{transactionId}/payment
{
  "paymentMethod": "CASH",
  "amount": 50.00,
  "receivedAmount": 60.00,
  "changeAmount": 10.00
}
```

**Step 4: Complete Transaction**
```http
POST /api/transaction/{transactionId}/complete
```

### 2. Inventory Management Flow

#### Purchase Order Process:
1. **Create Purchase Order** → 2. **Add Items** → 3. **Approve** → 4. **Receive Stock** → 5. **Update Inventory**

#### Stock Transfer Process:
1. **Create Transfer** → 2. **Approve** → 3. **Ship** → 4. **Receive** → 5. **Complete**

### 3. Product Management Flow

#### Adding New Product:
1. **Create Product** → 2. **Upload Image** → 3. **Set Stock Levels** → 4. **Activate**

## API Endpoints Reference

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | User login | No |
| POST | `/api/auth/register` | User registration | No |
| POST | `/api/auth/logout` | User logout | Yes |
| POST | `/api/auth/refresh-token` | Refresh JWT token | No |
| POST | `/api/auth/change-password` | Change user password | Yes |
| POST | `/api/auth/reset-password` | Request password reset | No |
| GET | `/api/auth/current-user` | Get current user info | Yes |
| POST | `/api/auth/validate-token` | Validate JWT token | No |

### Product Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/product` | Get all products | Yes |
| GET | `/api/product/{id}` | Get product by ID | Yes |
| GET | `/api/product/by-barcode/{barcode}` | Get product by barcode | Yes |
| GET | `/api/product/search?searchTerm={term}` | Search products | Yes |
| GET | `/api/product/low-stock` | Get low stock products | Yes |
| POST | `/api/product` | Create new product | Yes |
| PUT | `/api/product/{id}` | Update product | Yes |
| DELETE | `/api/product/{id}` | Delete product | Yes |

#### Product Request Models:

**CreateProductRequest:**
```json
{
  "productName": "Product Name",
  "productCode": "PRD001",
  "description": "Product description",
  "barcode": "1234567890",
  "unitPrice": 25.00,
  "costPrice": 15.00,
  "stockQuantity": 100,
  "minStockLevel": 10,
  "categoryId": 1,
  "image": "file_upload"  // multipart/form-data
}
```

### Transaction Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/transaction` | Create new transaction | Yes |
| GET | `/api/transaction/{id}` | Get transaction by ID | Yes |
| GET | `/api/transaction/by-number/{number}` | Get transaction by number | Yes |
| GET | `/api/transaction/by-date-range` | Get transactions by date range | Yes |
| GET | `/api/transaction/held/{storeId}` | Get held transactions | Yes |
| POST | `/api/transaction/{id}/items` | Add item to transaction | Yes |
| DELETE | `/api/transaction/{id}/items/{itemId}` | Remove item from transaction | Yes |
| POST | `/api/transaction/{id}/payment` | Process payment | Yes |
| POST | `/api/transaction/{id}/complete` | Complete transaction | Yes |
| POST | `/api/transaction/{id}/hold` | Hold transaction | Yes |
| POST | `/api/transaction/{id}/void` | Void transaction | Yes |

### Customer Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/customer` | Get all customers | Yes |
| GET | `/api/customer/{id}` | Get customer by ID | Yes |
| GET | `/api/customer/search?term={searchTerm}` | Search customers | Yes |
| POST | `/api/customer` | Create new customer | Yes |
| PUT | `/api/customer/{id}` | Update customer | Yes |
| DELETE | `/api/customer/{id}` | Delete customer | Yes |

### Inventory & Stock Management

#### Purchase Orders
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/purchaseorder` | Get all purchase orders | Yes |
| GET | `/api/purchaseorder/{id}` | Get purchase order by ID | Yes |
| GET | `/api/purchaseorder/supplier/{supplierId}` | Get POs by supplier | Yes |
| GET | `/api/purchaseorder/pending` | Get pending purchase orders | Yes |
| POST | `/api/purchaseorder` | Create purchase order | Yes |
| PUT | `/api/purchaseorder/{id}` | Update purchase order | Yes |
| POST | `/api/purchaseorder/{id}/approve` | Approve purchase order | Yes |
| POST | `/api/purchaseorder/{id}/cancel` | Cancel purchase order | Yes |

#### Stock Receiving
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/stockreceiving` | Get all stock receivings | Yes |
| GET | `/api/stockreceiving/{id}` | Get stock receiving by ID | Yes |
| GET | `/api/stockreceiving/pending` | Get pending receivings | Yes |
| POST | `/api/stockreceiving` | Create stock receiving | Yes |
| POST | `/api/stockreceiving/{id}/complete` | Complete receiving | Yes |
| POST | `/api/stockreceiving/{id}/process` | Process received items | Yes |

#### Stock Movement
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/stockmovement` | Get stock movements | Yes |
| GET | `/api/stockmovement/product/{productId}` | Get movements by product | Yes |
| POST | `/api/stockmovement/record` | Record stock movement | Yes |

### Reporting

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/report/daily-sales` | Daily sales report | Yes |
| GET | `/api/report/product-sales` | Product sales report | Yes |
| GET | `/api/report/cashier-performance` | Cashier performance report | Yes |
| GET | `/api/report/top-selling-products` | Top selling products | Yes |
| GET | `/api/report/profit-margin` | Profit margin report | Yes |
| GET | `/api/report/stock-movement` | Stock movement report | Yes |
| GET | `/api/report/sales-summary` | Sales summary | Yes |
| GET | `/api/report/dashboard-metrics` | Dashboard metrics | Yes |

### System Management

#### Categories
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/category` | Get all categories | Yes |
| GET | `/api/category/{id}` | Get category by ID | Yes |
| POST | `/api/category` | Create category | Yes |
| PUT | `/api/category/{id}` | Update category | Yes |
| DELETE | `/api/category/{id}` | Delete category | Yes |

#### Suppliers
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/supplier` | Get all suppliers | Yes |
| GET | `/api/supplier/{id}` | Get supplier by ID | Yes |
| POST | `/api/supplier` | Create supplier | Yes |
| PUT | `/api/supplier/{id}` | Update supplier | Yes |
| DELETE | `/api/supplier/{id}` | Delete supplier | Yes |

#### Stores & Warehouses
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/store` | Get all stores | Yes |
| GET | `/api/warehouse` | Get all warehouses | Yes |
| POST | `/api/store` | Create store | Yes |
| POST | `/api/warehouse` | Create warehouse | Yes |

## Data Models

### Core Models

#### Product
```json
{
  "id": 1,
  "productCode": "PRD001",
  "productName": "Sample Product",
  "description": "Product description",
  "barcode": "1234567890",
  "unitPrice": 25.00,
  "costPrice": 15.00,
  "stockQuantity": 100,
  "minStockLevel": 10,
  "categoryId": 1,
  "category": {
    "id": 1,
    "categoryName": "Electronics"
  },
  "imageUrl": "/images/products/product1.jpg",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### Transaction
```json
{
  "id": 1,
  "transactionNumber": "TXN-20240101-001",
  "storeId": 1,
  "customerId": 123,
  "userId": 456,
  "status": "COMPLETED",
  "subTotal": 50.00,
  "taxAmount": 5.50,
  "discountAmount": 2.50,
  "totalAmount": 53.00,
  "items": [
    {
      "id": 1,
      "productId": 1,
      "product": { /* Product object */ },
      "quantity": 2,
      "unitPrice": 25.00,
      "lineTotal": 50.00,
      "discountAmount": 0
    }
  ],
  "payments": [
    {
      "id": 1,
      "paymentMethod": "CASH",
      "amount": 53.00,
      "receivedAmount": 60.00,
      "changeAmount": 7.00,
      "status": "COMPLETED"
    }
  ],
  "createdAt": "2024-01-01T12:00:00Z"
}
```

#### Customer
```json
{
  "id": 1,
  "customerCode": "CUST001",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St",
  "dateOfBirth": "1990-01-01",
  "loyaltyPoints": 150,
  "totalSpent": 1250.00,
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

## Error Handling

### Standard Error Response Format
```json
{
  "message": "Error description",
  "error": "Detailed error information",
  "statusCode": 400
}
```

### Common HTTP Status Codes
- **200 OK**: Successful request
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

### Validation Errors
```json
{
  "message": "Validation failed",
  "errors": {
    "ProductName": ["Product name is required"],
    "UnitPrice": ["Unit price must be greater than 0"]
  }
}
```

## Best Practices

### 1. Authentication
- Always check token expiration before making API calls
- Implement automatic token refresh
- Store tokens securely (not in localStorage for sensitive apps)
- Handle 401 responses by redirecting to login

### 2. Error Handling
- Implement global error handling
- Show user-friendly error messages
- Log errors for debugging
- Handle network failures gracefully

### 3. Data Management
- Implement proper loading states
- Cache frequently accessed data
- Use pagination for large datasets
- Validate data before sending to API

### 4. Performance
- Implement debouncing for search inputs
- Use lazy loading for images
- Minimize API calls with proper state management
- Implement offline capabilities where appropriate

### 5. Security
- Validate all user inputs
- Sanitize data before display
- Use HTTPS in production
- Implement proper CORS policies

### 6. Transaction Flow Best Practices

#### For POS Interface:
1. **Product Search**: Implement barcode scanning and quick search
2. **Cart Management**: Allow easy addition/removal of items
3. **Payment Processing**: Support multiple payment methods
4. **Receipt Generation**: Provide immediate receipt printing/email
5. **Transaction Hold**: Allow saving incomplete transactions

#### For Inventory Management:
1. **Real-time Updates**: Reflect stock changes immediately
2. **Low Stock Alerts**: Notify when products reach minimum levels
3. **Batch Operations**: Allow bulk updates for efficiency
4. **Audit Trail**: Track all inventory movements

### 7. Sample Frontend Implementation Patterns

#### React/Vue.js Example for Product Search:
```javascript
// Debounced search implementation
const useProductSearch = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(false);
  
  const searchProducts = useCallback(
    debounce(async (searchTerm) => {
      if (!searchTerm) return;
      
      setLoading(true);
      try {
        const response = await api.get(`/api/product/search?searchTerm=${searchTerm}`);
        setProducts(response.data);
      } catch (error) {
        handleError(error);
      } finally {
        setLoading(false);
      }
    }, 300),
    []
  );
  
  return { products, loading, searchProducts };
};
```

#### Transaction Management:
```javascript
const useTransaction = () => {
  const [currentTransaction, setCurrentTransaction] = useState(null);
  
  const createTransaction = async (storeId, customerId) => {
    const response = await api.post('/api/transaction', {
      storeId,
      customerId
    });
    setCurrentTransaction(response.data);
    return response.data;
  };
  
  const addItem = async (transactionId, item) => {
    const response = await api.post(`/api/transaction/${transactionId}/items`, item);
    setCurrentTransaction(response.data);
    return response.data;
  };
  
  const processPayment = async (transactionId, payment) => {
    const response = await api.post(`/api/transaction/${transactionId}/payment`, payment);
    setCurrentTransaction(response.data);
    return response.data;
  };
  
  return {
    currentTransaction,
    createTransaction,
    addItem,
    processPayment
  };
};
```

This documentation provides a comprehensive guide for frontend developers to implement all business processes in the POS system. Each endpoint includes the necessary request/response formats and the business logic flow required for proper implementation.