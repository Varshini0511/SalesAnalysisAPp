using SalesAnalysisApp.Entities;
using SalesAnalysisApp.Dtos;
namespace SalesAnalysisApp.Service
{
    public interface IDataRefreshService
    {
        Task<DataRefreshResponse> RefreshDatafromCsvAsync(string filePath, bool isFullRefresh);
        Task<DataRefreshResponse> GetDataRefreshStatusAsync(int logId);
    }
}
