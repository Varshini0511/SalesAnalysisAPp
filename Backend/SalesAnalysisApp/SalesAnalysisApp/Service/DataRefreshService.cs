using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Domain;
using SalesAnalysisApp.Dtos;
using SalesAnalysisApp.Entities;

namespace SalesAnalysisApp.Service
{
    public class DataRefreshService:IDataRefreshService
    {
        private readonly SalesDbContext _salesDbContext;
        //private readonly ILogger _logger;

        public DataRefreshService(SalesDbContext salesDbContext)
        {
            _salesDbContext = salesDbContext;
          //  _logger = logger;
        }

        public async Task<DataRefreshResponse> RefreshDatafromCsvAsync(string filePath, bool isFullRefresh)
        {
            var log = new DataRefreshLog
            {
                RefreshType = isFullRefresh ? "FULL" : "Incremental",
                Status = "Started",
                StartedAt = DateTime.Now
            };

            _salesDbContext.DataRefreshLogs.Add(log);
            await _salesDbContext.SaveChangesAsync();

            var records = await REadCsvFileAsync(filePath);

            if (isFullRefresh)
            {
                await PerformFullRefreshAsync(records, log);
            }
            log.Status = "Success";
            log.CompletedAt = DateTime.UtcNow;
            await _salesDbContext.SaveChangesAsync();
            //_logger.LogInformation("DAta refresh completed. Logid{0}", log.LogId);

            return new DataRefreshResponse
            {
                LogId = log.LogId,
                Message = "Data refresh completed",
                RecordsProcessed = log.RecordsProcessed,
            };

        }
        private async Task<List<SalesRecord>> REadCsvFileAsync(string filepath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };
            using var reader = new StreamReader(filepath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CsvSalesRecordMap>();
            return await Task.Run(() => csv.GetRecords<SalesRecord>().ToList());
        }

        private async Task PerformFullRefreshAsync(List<SalesRecord> records, DataRefreshLog log)
        {
            using var transaction = await _salesDbContext.Database.BeginTransactionAsync();

            try
            {
                _salesDbContext.OrderItems.RemoveRange(_salesDbContext.OrderItems);
                _salesDbContext.Orders.RemoveRange(_salesDbContext.Orders);
                _salesDbContext.Products.RemoveRange(_salesDbContext.Products);
                _salesDbContext.Customers.RemoveRange(_salesDbContext.Customers);
                _salesDbContext.Categories.RemoveRange(_salesDbContext.Categories);
                _salesDbContext.Regions.RemoveRange(_salesDbContext.Regions);
                _salesDbContext.PaymentMethods.RemoveRange(_salesDbContext.PaymentMethods);

                _salesDbContext.SaveChangesAsync();

                     await InsertDataAsync(records, log);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task InsertDataAsync(List<SalesRecord> records, DataRefreshLog log)
        {
            try {
                log.RecordsProcessed = records.Count;
                var categoryCache = new Dictionary<string, Category>();
                var regionCache = new Dictionary<string, Region>();

                var productCache = new Dictionary<int, Product>();
                var customerCache = new Dictionary<int, Customer>();
                var paymethodCache = new Dictionary<string, PaymentMethod>();
                var orderCache = new Dictionary<int, Order>();

                var inserted = 0;
                var failed = 0;

                foreach (var record in records)
                {


                    if (!categoryCache.ContainsKey(record.CategoryName))
                    {
                        var category = await _salesDbContext.Categories.
                            FirstOrDefaultAsync(c => c.CategoryName == record.CategoryName);
                        if (category == null)
                        {
                            category = new Category { CategoryName = record.CategoryName };
                            _salesDbContext.Categories.Add(category);
                            await _salesDbContext.SaveChangesAsync();
                        }
                        categoryCache[record.CategoryName] = category;
                    }


                    if (!regionCache.ContainsKey(record.RegionName))
                    {
                        var region = await _salesDbContext.Regions.
                            FirstOrDefaultAsync(c => c.RegionName == record.RegionName);
                        if (region == null)
                        {
                            region = new Region { RegionName = record.RegionName };
                            _salesDbContext.Regions.Add(region);
                            await _salesDbContext.SaveChangesAsync();
                        }
                        regionCache[record.RegionName] = region;
                    }

                    if (!paymethodCache.ContainsKey(record.PaymentMethodName))
                    {
                        var paymentmethod = await _salesDbContext.PaymentMethods.
                            FirstOrDefaultAsync(c => c.PaymentMethodName == record.PaymentMethodName);
                        if (paymentmethod == null)
                        {
                            paymentmethod = new PaymentMethod { PaymentMethodName = record.PaymentMethodName };
                            _salesDbContext.PaymentMethods.Add(paymentmethod);
                            await _salesDbContext.SaveChangesAsync();
                        }
                        paymethodCache[record.PaymentMethodName] = paymentmethod;
                    }

                    if (!customerCache.ContainsKey(record.CustomerId))
                    {
                        var customer = await _salesDbContext.Customers.
                            FirstOrDefaultAsync(c => c.CustomerId == record.CustomerId);
                        if (customer == null)
                        {
                            customer = new Customer { CustomerId = record.CustomerId, CustomerName = record.CustomerName };
                            _salesDbContext.Customers.Add(customer);

                        }
                        else
                        {
                            customer.CustomerName = record.CustomerName;
                            customer.LastUpdatedAt = DateTime.UtcNow;

                        }
                        await _salesDbContext.SaveChangesAsync();
                        customerCache[record.CustomerId] = customer;

                    }
                    if (!productCache.ContainsKey(record.ProductId))
                    {
                        var product = await _salesDbContext.Products.
                            FirstOrDefaultAsync(c => c.ProductId == record.ProductId);
                        if (product == null)
                        {
                            product = new Product
                            { ProductId = record.ProductId,
                                ProductName = record.ProductName,
                                CategoryId = categoryCache[record.CategoryName].CategoryId,
                                UnitPrice = record.UnitPrice,
                            };
                            _salesDbContext.Products.Add(product);

                        }
                        else
                        {
                            product.ProductName = record.ProductName;
                            product.LastUpdatedAt = DateTime.UtcNow;
                            product.UnitPrice = record.UnitPrice;
                            product.CategoryId = categoryCache[record.CategoryName].CategoryId;

                        }
                        await _salesDbContext.SaveChangesAsync();
                        productCache[record.CustomerId] = product;


                    }
                    if (!orderCache.ContainsKey(record.OrderId))
                    {
                        var order = await _salesDbContext.Orders.
                            FirstOrDefaultAsync(c => c.OrderId == record.OrderId);
                        if (order == null)
                        {
                            order = new Order
                            {
                                OrderId = record.OrderId,
                                CustomerId = record.CustomerId,
                                RegionId = regionCache[record.RegionName].RegionId,
                                PaymentMethodId = paymethodCache[record.PaymentMethodName].PaymentMethodId,
                                OrderDate = record.DateOfSale,
                                ShippingCost = record.ShippingCOst

                            };
                            _salesDbContext.Orders.Add(order);
                            await _salesDbContext.SaveChangesAsync();

                        }


                        orderCache[record.OrderId] = order;

                    }
                    var orderItem = new OrderItem
                    {
                        OrderId = record.OrderId,
                        ProductId = record.ProductId,
                        QuantitySold = record.QuantitySold,
                        UnitPrice = record.UnitPrice,
                        Discount = record.Discount,
                    };
                    _salesDbContext.OrderItems.Add(orderItem);
                    await _salesDbContext.SaveChangesAsync();

                    inserted++;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError("Error Processing the record");

            }
        }

        public async Task<DataRefreshResponse> GetDataRefreshStatusAsync(int logId)
        {
            var log = await _salesDbContext.DataRefreshLogs.FindAsync(logId);
            if (log == null)
            {
                return new DataRefreshResponse
                {
                    // Status="Not Found",
                    Message = "Refresh log not found"
                };
            }
            return new DataRefreshResponse
            {
                LogId = log.LogId,
                Message = log.ErrorMessage ?? "Data refresh inprogress",
                RecordsProcessed = log.RecordsProcessed
            };
    }
    }



    public class CsvSalesRecordMap : ClassMap<SalesRecord>
    {
        public CsvSalesRecordMap()
        {
            Map(m => m.OrderId).Name("OrderID", "Order Id", "OrderId");
            Map(m => m.ProductId).Name("ProductId", "Product Id", "ProductId");
            Map(m => m.CustomerId).Name("CustomerID", "Customer Id", "CustomerId");
            Map(m => m.ProductName).Name("ProductName", "Product Name");

            Map(m => m.CategoryName).Name("CategoryName", "Category Name");
            Map(m => m.RegionName).Name("RegionName", "Region Name");

            Map(m => m.DateOfSale).Name("DateOfSale", "Date Of Sale");

            Map(m => m.QuantitySold).Name("QuantitySold", "Quantity Sold");

            Map(m => m.UnitPrice).Name("UnitPrice", "Unit Price");

            Map(m => m.Discount).Name("Discount");

            Map(m => m.ShippingCOst).Name("ShippingCOst", "Shipping COst");
            Map(m => m.PaymentMethodName).Name("PaymentMethodName", "Payment Method Name Id");
            Map(m => m.CustomerName).Name("Customer Name", "CustomerName");
        } } }






        
  

