using SalesAnalysisApp.Domain;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Entities;
using SalesAnalysisApp.Dtos;

namespace SalesAnalysisApp.Repository
{
    public interface ISalesRepository
    {
      Task<decimal> GetTotalRevenueAsync ( DateTime? startDate, DateTime? endDate );
        Task<List<RevenueByTypeResponse>> GetREvenueByProductAsync(DateTime? startDate, DateTime? endDate);
        Task<List<RevenueByTypeResponse>> GetRevenueByCategoryAsync(DateTime? startDate, DateTime? endDate);
        Task<List<RevenueByTypeResponse>> GetREvenueByRegionAsync(DateTime? startDate, DateTime? endDate);

        Task<List<TopProductResponse>> GetTopProductAsync(int top , string categoryName=null, string regionName=null );

        Task<CustomerReportResponse> GetCustomerReportAnalysisAsync(DateTime? startDate, DateTime? endDate);


    }
}
