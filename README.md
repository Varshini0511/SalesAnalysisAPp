Endpoint	Description
POST /api/datarefresh/trigger	Import CSV data
GET  /api/DataREfresh/GetRefreshStatus to get REfresh status
GET /api/SalesAnalysis/revenue/total	Total revenue
GET /api/SalesAnalysis/revenue/byProduct	Revenue by product
GET /api/SalesAnalysis/revenue/byCategory	Revenue by category
GET /api/SalesAnalysis/revenue/byRegion 	Revenue by region
GET /api/SalesAnalysis/revenue/getTopProduct	Top N products
GET /api/SalesAnalysis/revenue/getCustomerReport	Customer metrics


CSV Format Required
OrderID, ProductID, CustomerID, ProductName, Category, Region, DateOfSale, QuantitySold, UnitPrice, Discount, ShippingCost, PaymentMethod, CustomerName

Features
 Normalized 3NF database schema
 CSV import with validation
 Revenue analytics (total, by product/category/region)
 Top products analysis
 Customer insights
 Daily automated refresh
 Comprehensive error handling

CSV Required columns:

OrderID, ProductID, CustomerID
ProductName, Category, Region
DateOfSale (DateTime)
QuantitySold (int)
UnitPrice, Discount, ShippingCost (decimal)
PaymentMethod, CustomerName

Database Schema
Tables
Categories - Product categories
Regions - Sales regions
PaymentMethods - Payment types
Customers - Customer information
Products - Product catalog
Orders - Order headers
OrderItems - Order line items
DataRefreshLogs - Import logs

Key Features
Foreign key constraints
Indexes on frequently queried columns
Computed column for line totals
Cascade delete for order items
Background Services

Optional daily data refresh runs at midnight:
Configured via DataRefreshBackgroundService
Set CSV path in environment variable: SALES_DATA_CSV_PATH
Error Handling
Comprehensive try-catch blocks
Detailed error logging
Transaction rollback on failures
Validation at multiple levels

Performance Optimizations
Async/await throughout
Proper indexing strategy
Connection pooling
Efficient LINQ queries