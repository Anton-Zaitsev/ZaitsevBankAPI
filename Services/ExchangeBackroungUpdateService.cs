using System.Globalization;

namespace ZaitsevBankAPI.Services
{
    public class ExchangeBackroungUpdateService : BackgroundService
    {
        private readonly ILogger _logger;

        public ExchangeBackroungUpdateService(ILogger<ExchangeBackroungUpdateService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                    ExchangesService exchangesService = new();
                    _ = await exchangesService.ExchangesUpdate();
                }
                catch(Exception e)
                {
                    _logger.LogError("ExchangeBackroungUpdateService/ExecuteAsync",e.Message);
                }

                await Task.Delay(21600000, stoppingToken); // Обновление раз в 6 часов
            }
        }
    }
}
