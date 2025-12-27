using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesAnalysisApp.Repository;
using SalesAnalysisApp.Dtos;
using SalesAnalysisApp.Entities;
namespace SalesAnalysisApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesAnalysisController : ControllerBase
    {
        public readonly ISalesRepository _salesRepository;
        public readonly ILogger _logger;

        public SalesAnalysisController(ISalesRepository salesRepository, ILogger logger)
        {
            _salesRepository = salesRepository;
            _logger = logger;
        }

        [HttpGet("revenue/total")]
        public async Task<ActionResult<ApiResponse<RevenueResponse>>> GetTotalRevenue(inputRequest request)
        {
            var revenue = await _salesRepository.GetTotalRevenueAsync(request.startDate, request.endDate);
            if (revenue != null)
                return Ok(new ApiResponse<RevenueResponse>
                {
                    Success = true,
                    Message = "Total Revenue retrieved",
                    Data = new RevenueResponse
                    {
                        TotalRevenue = revenue,
                        StartDate = request.startDate,
                        EndDate = request.endDate
                    }
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving total revenue"

                });

            }

        }

        [HttpGet("revenue/byProduct")]
        public async Task<ActionResult<ApiResponse<object>>> GetRevenueByProduct(inputRequest request)
        {
            var data = await _salesRepository.GetREvenueByProductAsync(request.startDate, request.endDate);
            if (data != null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Revenue retrieved by Product",
                    Data = new
                    {
                        Product = data,
                        Count = data.Count

                    }
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving revenue by product"

                });

            }

        }

        [HttpGet("revenue/byRegion")]
        public async Task<ActionResult<ApiResponse<object>>> GetRevenueByRegion(inputRequest request)
        {
            var data = await _salesRepository.GetREvenueByRegionAsync(request.startDate, request.endDate);
            if (data != null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = " Revenue retrieved by Region",
                    Data = new
                    {
                        Product = data,
                        Count = data.Count

                    }
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving revenue by region"

                });

            }

        }


        [HttpGet("revenue/byCategory")]
        public async Task<ActionResult<ApiResponse<object>>> GetRevenueByCategory(inputRequest request)
        {
            var data = await _salesRepository.GetRevenueByCategoryAsync(request.startDate, request.endDate);
            if (data != null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Revenue retrieved by category",
                    Data = new
                    {
                        Product = data,
                        Count = data.Count

                    }
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving revenue by category"

                });

            }

        }
        [HttpGet("revenue/getTopProduct")]
        public async Task<ActionResult<ApiResponse<object>>> GetTopProducts(int top, string category, string region)
        {
            if (top <= 0)
                return BadRequest();
            var data = await _salesRepository.GetTopProductAsync(top, category, region);
            if (data != null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Top products by revenue retrieved",
                    Data = new
                    {
                        Product = data,
                        Count = data.Count,
                        FilteredbyCategory = category,
                        FilteredByRegion = region

                    }
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving top products"

                });

            }

        }
        [HttpGet("revenue/getCustomerReport")]
        public async Task<ActionResult<ApiResponse<CustomerReportResponse>>> GetCustomerReport(inputRequest request)
        {
            var data = await _salesRepository.GetCustomerReportAnalysisAsync(request.startDate, request.endDate);
            if (data != null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "CustomerReport retrieved",
                    Data = data
                });
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving customer reprot"

                });

            }



        }
    }
}
