using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ZaitsevBankAPI.Models;
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

            TransactionsTransferService transactionsTransferService = new();
            bool completed = await transactionsTransferService.TransferToClient(TransactionSender, TransactionRecipient, Summ);
            return completed ? Ok() : StatusCode(412, "Не удалось перевести деньги клиенту");

        }

        [HttpPost]
        public async Task<IActionResult> ValuteBuySale(string CardA, string CardB, string Summ, bool BuySale)
        {
            TransactionsValute transactionsValute = new();
            bool completed = await transactionsValute.ValuteBuySale(CardA, CardB, Summ, BuySale);
            return completed ? Ok() : StatusCode(412, "Не удалось купить/продать валюту");
        }

        [HttpGet]
        public async Task<IActionResult> CheckCredit(string count, string year)
        {
            TransactionCreditService transactionCreditService = new();
            var credit = transactionCreditService.creditCheck(count, year);
            return credit != null ? Ok(credit) : StatusCode(412, "Не оформить кредит");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCredit(string count, string year, string transactionCard)
        {
            TransactionCreditService transactionCreditService = new();
            bool completed = await transactionCreditService.ApplyCredit(count, year, transactionCard);
            return completed ? Ok() : StatusCode(412, "Не оформить кредит");
        }

        [HttpPost]
        public async Task<IActionResult> AddMoneyCredit(string transactionCard,string creditID)
        {
            TransactionCreditService transactionCreditService = new();
            var completed = await transactionCreditService.AddMoneyCredit(transactionCard, creditID);
            return completed ? Ok() : StatusCode(412, "Не оформить кредит");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransaction(string userID,string dataFrom, string dataTo)
        {
            Guid id = Guid.Parse(userID);
            DateTime DateFrom;
            DateTime DateTo;
            if (!DateTime.TryParseExact(dataFrom, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateFrom)) return NotFound();
            if (!DateTime.TryParseExact(dataTo, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTo)) return NotFound();
            DateTo = DateTo.AddHours(23);
            DateTo = DateTo.AddMinutes(59);
            DateTo = DateTo.AddSeconds(59);
            DateTo = DateTo.AddMilliseconds(999);

            TransactionsGetList transactionsGetList = new();   
            List<Task<List<AllTransactions>?>> listTask = new();

            listTask.Add(transactionsGetList.GetAllCardsTransactions(DateFrom, DateTo, id));
            listTask.Add(transactionsGetList.GetAllTransferTransactions(DateFrom, DateTo, id));
            listTask.Add(transactionsGetList.GetAllCreditsTransaction(DateFrom, DateTo, id));
            listTask.Add(transactionsGetList.GetAllCurrencyTransaction(DateFrom, DateTo, id));
            await Task.WhenAll(listTask);

            List<AllTransactions> allTransactions = new();
            foreach (Task<List<AllTransactions>?>? task in listTask)
            {
                if (task.Result != null)
                {
                    allTransactions.AddRange(task.Result);  
                }
            }
            return allTransactions.Count > 0 ? Ok(allTransactions) : NotFound();
        }

    }
}
