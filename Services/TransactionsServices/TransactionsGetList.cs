using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Models.TransactionsModels;
using static ZaitsevBankAPI.FunctionBank.Operation;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
	public class TransactionsGetList
	{

        public async Task<List<AllTransactions>?> GetAllTransferTransactions(DateTime dateIN, DateTime dateFrom, Guid userID)
        {
            int operationIncomingTransfer = (int)OperationNumber.IncomingTransfer;
            int operationOutgoingTransfer = (int)OperationNumber.OutgoingTransfer;
            int operationBetweenCards = (int)OperationNumber.BetweenMyCards;

            List<AllTransactions> list = new();

            using ApplicationContext _context = new();
            var getTransaction = await _context.TransferClient.Include(x => x.Transactions).Where(x => (x.UserSender == userID || x.UserRecipient == userID)
            && (x.Transactions.Where(y => y.ArrivalDate >= dateIN && y.ArrivalDate <= dateFrom).ToList().Count > 0)).ToListAsync();
            if (getTransaction.Count == 0) return null;

            await _context.DisposeAsync();
            foreach (TransferClient transferClient in getTransaction)
            {
                
                
            }
            return list;

        }
        private async Task<AllTransactions?> GenerateTransferTransaction(TransferClient transferClient)
        {
            AllTransactions transactions = new();

            if (transferClient.CurrencyTransferID == null)
            {
                return transactions;
            }
            else
            {
                using ApplicationContext _context = new();
                var currencyTransaction = await _context.CurrencyTransfers.FirstOrDefaultAsync(x => x.TransactionsID == transferClient.CurrencyTransferID);
                if (currencyTransaction == null) return null;
                TransactionValute transactionValute = new()
                {

                };
                return transactions;
            }
        }
        public async Task<List<AllTransactions>?> GetAllCardsTransactions(DateTime dateIN, DateTime dateFrom, Guid userID)
        {
            int operationActivationCard = (int)OperationNumber.ActivationCard;
            int operationDeActivationCard = (int)OperationNumber.DeActivationCard;

            List<AllTransactions> list = new();
            using ApplicationContext _context = new();
            var getTransaction = await _context.PaymentServices.Include(x => x.Transactions).Where(x => x.UserID == userID
            && (x.Transactions.CodeOperation == operationActivationCard || x.Transactions.CodeOperation == operationDeActivationCard)
            && (x.Transactions.ArrivalDate >= dateIN && x.Transactions.ArrivalDate <= dateFrom)).ToListAsync();
            if (getTransaction.Count == 0) return null;

            foreach (PaymentServices transaction in getTransaction)
            {
                TransactionCardOrCredit transactionCard = new()
                {
                    Name = transaction.NameServices,
                    Activation = operationActivationCard == transaction.Transactions.CodeOperation
                };
                AllTransactions allTransactions = new()
                {
                    TypeTransaction = transaction.Transactions.CodeOperation,
                    NameTransaction = transaction.Transactions.NameOperation,
                    DateTime = transaction.Transactions.ArrivalDate.Value,
                    TransactionId = transaction.TransactionsID,
                    TransactionCardOrCredit = transactionCard

                };
                list.Add(allTransactions);
            }
            GC.Collect();
            return list;
        }
    }
}

