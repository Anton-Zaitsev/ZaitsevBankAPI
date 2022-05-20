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
            bool completed = await transactionsService.GetTransaction(userID);
            return completed ? Ok() : StatusCode(412, "Не удалось создать вашу карту");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTransaction(string userID)
        {
            Guid id = Guid.Parse(userID);
            TransactionsGetList transactionsGetList = new();
            DateTime dateTime2 = DateTime.Now;
            DateTime dateTime1 = dateTime2.AddMonths(-1);

            Task<List<AllTransactions>?> getCardTransaction = transactionsGetList.GetAllCardsTransactions(dateTime1, dateTime2, id);
            Task<List<AllTransactions>?> getTransferTransaction = transactionsGetList.GetAllTransferTransactions(dateTime1, dateTime2, id);
            await Task.WhenAll(getCardTransaction, getTransferTransaction);
            return getCardTransaction.Result != null ? Ok(getCardTransaction.Result) : NotFound();
        }

    }
}
