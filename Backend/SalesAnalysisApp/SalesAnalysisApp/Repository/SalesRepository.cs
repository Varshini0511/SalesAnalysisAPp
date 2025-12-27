using SalesAnalysisApp.Domain;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Entities;
using SalesAnalysisApp.Dtos;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SalesAnalysisApp.Repository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly SalesDbContext _salesDbContext;

        public SalesRepository(SalesDbContext salesDbContext)
        {
            _salesDbContext = salesDbContext;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _salesDbContext.OrderItems.Include(oi => oi.Order).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value);

            var revenue = await query.SumAsync(oi => oi.LineTotal);
            var shippingCosts = await _salesDbContext.Orders
                .Where(o => (!startDate.HasValue || o.OrderDate >= startDate.Value) &&
                (!endDate.HasValue || o.OrderDate <= endDate.Value))
                .SumAsync(c => c.ShippingCost);

            return revenue + shippingCosts;
        }
        public async Task<List<RevenueByTypeResponse>> GetREvenueByProductAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _salesDbContext.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value);

            return await query.GroupBy(oi => new { oi.ProductId, oi.Product.ProductName })
                .Select(g => new RevenueByTypeResponse
                {
                    Type = "Product",
                    TypeValue = g.Key.ProductName,
                    TotalREvenue = g.Sum(oi => oi.LineTotal)

                })
                .OrderByDescending(r => r.TotalREvenue)
                .ToListAsync();

        }

        public async Task<List<RevenueByTypeResponse>> GetREvenueByRegionAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _salesDbContext.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(oi => oi.Region)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value);

            return await query.GroupBy(oi => new { oi.Order.Region.RegionName })
                .Select(g => new RevenueByTypeResponse
                {
                    Type = "Region",
                    TypeValue = g.Key.RegionName,
                    TotalREvenue = g.Sum(oi => oi.LineTotal)

                })
                .OrderByDescending(r => r.TotalREvenue)
                .ToListAsync();

        }
        public async Task<List<RevenueByTypeResponse>> GetRevenueByCategoryAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _salesDbContext.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .ThenInclude(oi => oi.Category)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value);

            return await query.GroupBy(oi => new { oi.Product.Category.CategoryName })
                .Select(g => new RevenueByTypeResponse
                {
                    Type = "Category",
                    TypeValue = g.Key.CategoryName,
                    TotalREvenue = g.Sum(oi => oi.LineTotal)

                })
                .OrderByDescending(r => r.TotalREvenue)
                .ToListAsync();

        }
        public async Task<List<TopProductResponse>> GetTopProductAsync(int top, string categoryName = null, string regionName = null)
        {
            var query = _salesDbContext.OrderItems
                .Include(o => o.Product)
                .ThenInclude(p => p.Category)
                .Include(o => o.Order)
                .ThenInclude(o => o.Region)
                .AsQueryable();
            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(o => o.Product.Category.CategoryName == categoryName);
            if (!string.IsNullOrEmpty(regionName))
                query = query.Where(o => o.Order.Region.RegionName == regionName);

            return await query
                .GroupBy(o => new { o.ProductId, o.Product.ProductName, categoryName = o.Product.Category.CategoryName })
                .Select(g => new TopProductResponse
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryNAme = g.Key.categoryName,
                    TotalQuantitySold = g.Sum(o => o.QuantitySold),
                    TotalRevenue = g.Sum(o => o.LineTotal),

                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(top)
                .ToListAsync();
        }

        public async Task<CustomerReportResponse> GetCustomerReportAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _salesDbContext.Orders.AsQueryable();
            if (startDate.HasValue)
                query = query.Where(oi => oi.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(oi => oi.OrderDate <= endDate.Value);

            var orderId = await query.Select(o => o.OrderId).ToListAsync();

            var totalCustomers = await query
             .Select(o => o.CustomerId)
             .Distinct().CountAsync();

            var totalOrders = orderId.Count;
            var totalRevenue = await _salesDbContext.OrderItems.
                Where(o => orderId.Contains(o.OrderId))
                .SumAsync(o => o.LineTotal);

            var totalShipping = await query.SumAsync(o => o.ShippingCost);

            var GrandTotal = totalRevenue + totalShipping;
            return new CustomerReportResponse
            {
                TotalCustomers = totalCustomers,
                TotalOrdrs = totalOrders,
                AverageOrderValue = totalOrders > 0 ? GrandTotal / totalOrders : 0
            };


        }
        }

    }

