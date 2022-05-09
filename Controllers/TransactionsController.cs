using Microsoft.AspNetCore.Mvc;
using ZaitsevBankAPI.Services.TransactionsServices;

namespace ZaitsevBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        public TransactionsController(ILogger<TransactionsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> TransferClient(string TransactionSender, string TransactionRecipient, string Summ)
        {

            TransactionsService transactionsService = new();
            bool completed = await transactionsService.TransferToClient(TransactionSender, TransactionRecipient, Summ);
            return completed ? Ok() : StatusCode(412, "Не удалось перевести деньги клиенту");

        }

        [HttpPost]
        public async Task<IActionResult> ValuteBuySale(string CardA, string CardB, string summ, bool BuySale)
        {
            TransactionsService transactionsService = new();
            bool completed = await transactionsService.ValuteBuySale  (CardA, CardB, summ, BuySale);
            return completed ? Ok() : StatusCode(412, "Не удалось купить/продать валюту");
        }

        [HttpPost] 
        public async Task<IActionResult> GetTransaction(string userID)
        {
            TransactionsService transactionsService = new();
            bool completed = await transactionsService.GetTransaction(userID);
            return completed ? Ok() : StatusCode(412, "Не удалось создать вашу карту");
        }

    }
}
