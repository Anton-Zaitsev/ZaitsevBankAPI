using System.Collections.Generic;
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
        public async Task<IActionResult> ValuteBuySale(string CardA, string CardB, string summ, bool BuySale)
        {
            TransactionsValute transactionsValute = new();
            bool completed = await transactionsValute.ValuteBuySale(CardA, CardB, summ, BuySale);
            return completed ? Ok() : StatusCode(412, "Не удалось купить/продать валюту");
        }

        [HttpPost] 
        public async Task<IActionResult> GetTransaction(string userID)
        {
            TransactionsTransferService transactionsService = new();
            bool completed = true;
            return completed ? Ok() : StatusCode(412, "Не удалось создать вашу карту");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCredit(string count, string transactionCard)
        {
            TransactionCreditService transactionCreditService = new();
            var completed = await transactionCreditService.ApplyCredit(count, transactionCard);
            return completed ? Ok() : StatusCode(412, "Не оформить кредит");
        }

        [HttpPost]
        public async Task<IActionResult> AddMoneyCredit(string count, string transactionCard,string creditID)
        {
            TransactionCreditService transactionCreditService = new();
            var completed = await transactionCreditService.AddMoneyCredit(count, transactionCard, creditID);
            return completed ? Ok() : StatusCode(412, "Не оформить кредит");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransaction(string userID)
        {
            Guid id = Guid.Parse(userID);
            TransactionsGetList transactionsGetList = new();
            DateTime dateTime2 = DateTime.Now;
            DateTime dateTime1 = dateTime2.AddMonths(-1);

            List<Task<List<AllTransactions>?>> listTask = new();

            listTask.Add(transactionsGetList.GetAllCardsTransactions(dateTime1, dateTime2, id));
            listTask.Add(transactionsGetList.GetAllTransferTransactions(dateTime1, dateTime2, id));
            listTask.Add(transactionsGetList.GetAllCreditsTransaction(dateTime1, dateTime2, id));
            listTask.Add(transactionsGetList.GetAllCurrencyTransaction(dateTime1, dateTime2, id));
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
