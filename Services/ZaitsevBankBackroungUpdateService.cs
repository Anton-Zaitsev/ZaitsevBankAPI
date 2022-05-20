using System.Globalization;

namespace ZaitsevBankAPI.Services
{
    public class ZaitsevBankBackroungUpdateService : BackgroundService
    {
        private readonly ILogger _logger;

        public ZaitsevBankBackroungUpdateService(ILogger<ZaitsevBankBackroungUpdateService> logger)
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
                    Task<bool> task1 = new ExchangesService().ExchangesUpdate();
                    Task task2 = new TransactionsServices.TransactionsCardService().ClosedCards();
                    await Task.WhenAll(task1, task2);
                }
                catch (Exception e)
                {
                    _logger.LogError("ZaitsevBankBackroungUpdateService/ExecuteAsync", e.Message);
                }

                await Task.Delay(21600000, stoppingToken); // Обновление раз в 6 часов
            }
        }
    
    }
}
