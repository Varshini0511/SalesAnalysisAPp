using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace SalesAnalysisApp.Service
{
    public class DataRefreshBackgroundService: BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataRefreshBackgroundService> _logger;
        private Timer _timer;

            public DataRefreshBackgroundService(IServiceProvider serviceProvider, ILogger<DataRefreshBackgroundService> logger)
        {
            _serviceProvider = serviceProvider; 
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           _logger.LogInformation("DataREfresh Background service is started");

            var now=DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            var initialDelay = tomorrow - now;
            _timer = new Timer(DoWork,
                null,
                initialDelay,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Daily Data refresh started");
            using var scope = _serviceProvider.CreateScope();
            var dataREfreshService = scope.ServiceProvider.GetRequiredService<IDataRefreshService>();
            try
            {
                var filepath = Environment.GetEnvironmentVariable("path");
                if(!string.IsNullOrEmpty(filepath))
                {
                    await dataREfreshService.RefreshDatafromCsvAsync(filepath, isFullRefresh: false);
                    _logger.LogInformation("Daily Data refresh completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error occureed during daily data refresh ");

            }
        }
        public override void Dispose()
        {
            _timer.Dispose();
            base.Dispose();
        }

    }
}
