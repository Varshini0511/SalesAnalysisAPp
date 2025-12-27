using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesAnalysisApp.Service;
using SalesAnalysisApp.Dtos;
using SalesAnalysisApp.Entities;

namespace SalesAnalysisApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataREfreshController : ControllerBase
    {
        private readonly IDataRefreshService _dataRefreshService;

        public DataREfreshController(IDataRefreshService dataRefreshService)
        {
            _dataRefreshService = dataRefreshService;
                }
        [HttpPost("Trigger")]
        public async Task<ActionResult<ApiResponse<DataRefreshResponse>>> TriggerDataRefresh(DataRefreshREquest request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
            {
                return BadRequest(new ApiResponse<DataRefreshResponse>
                {
                    Success = false,
                    Message = "File PAth is required"

                });
}
            var result = await _dataRefreshService.RefreshDatafromCsvAsync(request.FilePath, request.IsFullRefresh);
            if (result != null)
            { return Ok(
                new ApiResponse<DataRefreshResponse>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            else
            {
                return BadRequest(new ApiResponse<DataRefreshResponse>
                {
                    Success = false,
                    Message = "Data REfresh failed"
                });
            
        }

        }

        [HttpGet("GetREfreshStatus")]
        public async Task<ActionResult<ApiResponse<DataRefreshResponse>>> GetREfreshStatus(int logId)
        {
            
                var result = await _dataRefreshService.GetDataRefreshStatusAsync(logId);
                if (result != null)
                {
                    return Ok(
                    new ApiResponse<DataRefreshResponse>
                    {
                        Success = true,
                        Message = result.Message,
                        Data = result
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<DataRefreshResponse>
                    {
                        Success = false,
                        Message = "REfresh log not found"
                    });

                }
            }
        }
    }

