using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Models.TransactionsModels;
using static ZaitsevBankAPI.FunctionBank.Operation;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
	public class TransactionsGetList
	{
        private readonly int operationActivationCard = (int)OperationNumber.ActivationCard;
        private readonly int operationDeActivationCard = (int)OperationNumber.DeActivationCard;

        private readonly int operationIncomingTransfer = (int)OperationNumber.IncomingTransfer; // Входящий
        private readonly int operationOutgoingTransfer = (int)OperationNumber.OutgoingTransfer; //Исходящий
        private readonly int operationBetweenCards = (int)OperationNumber.BetweenMyCards; // Между счетами

        private readonly int operationIncomingCurrencyTransfer = (int)OperationNumber.IncomingTransferAndCurrencyTransfer; //Входящий с переводом валюты
        private readonly int operationOutgoingCurrencyTransfer = (int)OperationNumber.OutgoingTransferAndCurrencyTransfer; //Исходящий с переводом валюты
        private readonly int operationBetweenCurrencyCards = (int)OperationNumber.BetweenMyCardsAndCurrencyTransfer; // Между своими с переводом валюты

        private readonly int operationTakeCredit = (int)OperationNumber.TakeCredit; // Взять кредит
        private readonly int operationPaymentCredit = (int)OperationNumber.PaymentCredit; // Оплата кредита
        private readonly int operationRepaymentCredit = (int)OperationNumber.RepaymentCredit; // Погащение кредита

        public async Task<List<AllTransactions>?> GetAllCurrencyTransaction(DateTime dateIN, DateTime dateFrom, Guid userID)
        {
            List<AllTransactions> list = new();
            using ApplicationContext _context = new();
            var getTransaction = await _context.CurrencyTransfers.Include(x => x.Transactions).Where(x => x.UserID == userID &&
            x.Transactions.ArrivalDate >= dateIN  && x.Transactions.ArrivalDate <= dateIN).ToListAsync();
            if (getTransaction == null) return null;
            await _context.DisposeAsync();
            List<Task<AllTransactions?>> taskTransaction = new();

            foreach (var taskGenerate in getTransaction)
            {
                taskTransaction.Add(GenerateCurrencyTransaction(taskGenerate));
            }
            await Task.WhenAll(taskTransaction);
            foreach (var task in taskTransaction)
            {
                if (task.Result != null)
                {
                    list.Add(task.Result);
                }
            }
            return list.Count > 0 ? list : null;
        }
        private async Task<AllTransactions?> GenerateCurrencyTransaction(CurrencyTransfer currencyTransfer)
        {
            using ApplicationContext _context = new();
            var cardA = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == currencyTransfer.TransactionCardFrom);
            if (cardA == null) return null;
            var cardB = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == currencyTransfer.TransactionCardTo);
            if (cardB == null) return null;
            if (cardA.UserID != cardB.UserID) return null; 
            TransactionValute transactionValute = new()
            {
                 BuySale = currencyTransfer.BuySaleValute,
                 CountValuteA = currencyTransfer.Transactions.Expenses.Value,
                 CountValuteB = currencyTransfer.Transactions.Arrival.Value,
                 ValuteA = currencyTransfer.ValuteA,
                 ValuteB = currencyTransfer.ValuteB
            };
            string FromInfoCard = cardA.NameCard + " •• " + cardA.NumberCard.Substring(cardA.NumberCard.Length - 4);
            string ToInfoCard = cardB.NameCard + " •• " + cardB.NumberCard.Substring(cardB.NumberCard.Length - 4);
            TransactionValuteBetween transactionValuteBetween = new()
            {
                 FromCardName = FromInfoCard,
                 ToCardName = ToInfoCard,
                 TransactionValute = transactionValute,
            };
            AllTransactions allTransactions = new()
            {
                TypeTransaction = currencyTransfer.Transactions.CodeOperation,
                NameTransaction = currencyTransfer.Transactions.NameOperation,
                DateTime = currencyTransfer.Transactions.ArrivalDate.Value,
                TransactionId = currencyTransfer.TransactionsID,
                TransactionValuteBetween = transactionValuteBetween
            };
            return allTransactions;
        }
        public async Task<List<AllTransactions>?> GetAllCreditsTransaction(DateTime dateIN, DateTime dateFrom, Guid userID)
        {
            List<AllTransactions> list = new();
            using ApplicationContext _context = new();
            var getTransaction = await _context.Credits.Include(x => x.Transactions).Where(x => x.UserID == userID && 
            (x.Transactions.Any(y => y.ArrivalDate >= dateIN && y.ArrivalDate <= dateFrom)) == true).ToListAsync();
            if (getTransaction == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserID == userID);
            if (user == null) return null;
            foreach (Credits credit in getTransaction)
            {
  
                foreach (Transactions transaction in credit.Transactions)
                {
                    if (transaction.CodeOperation == operationTakeCredit)
                    {
                        string lastName = user.MiddleName == null ? user.LastName : $"{user.LastName.First()}.";

                        TransactionPaymentServices transactionPaymentServices = new()
                        {
                            NameClient = lastName,
                            CountMoney = credit.CreditSumm,
                            ValuteType = transaction.ValuteTransactions
                        };

                        AllTransactions allTransactions = new()
                        {
                            TypeTransaction = transaction.CodeOperation,
                            NameTransaction = transaction.NameOperation,
                            DateTime = transaction.ArrivalDate.Value,
                            TransactionId = transaction.TransactionsID,
                            TransactionPaymentServices = transactionPaymentServices
                        };
                        list.Add(allTransactions);
                    }
                    else if (transaction.CodeOperation == operationPaymentCredit || transaction.CodeOperation == operationRepaymentCredit)
                    {
                        TransactionCredit transactionCredit = new()
                        {
                            NumberDocument = credit.NumberDocument.ToString(),
                            CountMoney = transaction.CodeOperation == operationRepaymentCredit ? credit.CreditSumm : transaction.Expenses.Value,
                            Progress = transaction.CodeOperation == operationRepaymentCredit ? 1 : (float)Math.Round(((transaction.Expenses.Value * 100) / credit.CreditSumm) / 100,4)
                                
                        };
                        AllTransactions allTransactions = new()
                        {
                            TypeTransaction = transaction.CodeOperation,
                            NameTransaction = transaction.NameOperation,
                            DateTime = transaction.ArrivalDate.Value,
                            TransactionId = transaction.TransactionsID,
                            TransactionCredit = transactionCredit
                        };
                        list.Add(allTransactions);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }
        public async Task<List<AllTransactions>?> GetAllTransferTransactions(DateTime dateIN, DateTime dateFrom, Guid userID)
        {

            List<AllTransactions> list = new();

            using ApplicationContext _context = new();
            var getTransaction = await _context.TransferClient.Include(x => x.Transactions).Where(x => (x.UserSender == userID || x.UserRecipient == userID)
            && (x.Transactions.Any(y => y.ArrivalDate >= dateIN && y.ArrivalDate <= dateFrom) == true)).ToListAsync();
            if (getTransaction.Count == 0) return null;

            await _context.DisposeAsync();

            List<Task<AllTransactions?>> listTask = new();

            foreach (var taskGenerate in getTransaction)
            {
                listTask.Add(GenerateTransferTransaction(taskGenerate, userID));
            }
            await Task.WhenAll(listTask);

            foreach (var task in listTask)
            {
                if (task.Result != null)
                {
                    list.Add(task.Result);
                }
            }
            return list;

        }
        private async Task<AllTransactions?> GenerateTransferTransaction(TransferClient transferClient,Guid userID)
        {
            
            AllTransactions transactions = new();


            if (transferClient.CurrencyTransferID == null)
            {
                // Проверяем, это транзакция на входящий или исходящие валюты ?
                if (transferClient.Transactions.All(x => x.CodeOperation == operationIncomingTransfer || x.CodeOperation == operationOutgoingTransfer) == true)
                {
                    TransactionPaymentServices transactionPaymentServices = new();
                    

                    if (transferClient.UserRecipient == userID) // Наш человек получатель?
                    {
                        Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationIncomingTransfer); // Ищем входящий перевод
                        if (listCurrentle == null) return null;
                        // Заполняем модель оплаты 
                        transactionPaymentServices.NameClient = transferClient.NameSender;
                        transactionPaymentServices.CountMoney = listCurrentle.Arrival.Value;
                        transactionPaymentServices.ValuteType = listCurrentle.ValuteTransactions;
                        // Заполняем модель трансфера

                        transactions.TypeTransaction = listCurrentle.CodeOperation; 
                        transactions.NameTransaction = listCurrentle.NameOperation;
                        transactions.TransactionId = transferClient.TransactionsID;
                        transactions.DateTime = listCurrentle.ArrivalDate.Value; // Дата доставки транзакции
                        transactions.TransactionPaymentServices = transactionPaymentServices; // Добавляем транзакцию оплаты
                        GC.Collect();
                        return transactions;
                    }
                    else // Наш человек отправитель!
                    {
                        Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationOutgoingTransfer); // Ищем исходящий перевод
                        if (listCurrentle == null) return null;
                        // Заполняем модель оплаты 
                        transactionPaymentServices.NameClient = transferClient.NameRecipient;
                        transactionPaymentServices.CountMoney = listCurrentle.Expenses.Value;
                        transactionPaymentServices.ValuteType = listCurrentle.ValuteTransactions;
                        // Заполняем модель трансфера

                        transactions.TypeTransaction = listCurrentle.CodeOperation;
                        transactions.NameTransaction = listCurrentle.NameOperation;
                        transactions.TransactionId = transferClient.TransactionsID;
                        transactions.DateTime = listCurrentle.ExpensesDate.Value; // Дата принятия валюты
                        transactions.TransactionPaymentServices = transactionPaymentServices;// Добавляем транзакцию оплаты
                        GC.Collect();
                        return transactions;

                    }
                }
                else if (transferClient.Transactions.All(x => x.CodeOperation == operationBetweenCards) == true)
                {
                    using ApplicationContext _context = new();
                    Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationBetweenCards); // Ищем перевод между счетами
                    if (listCurrentle == null) return null;
                    var cardSender = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == transferClient.TransactionSender);
                    if (cardSender == null) return null;
                    var cardRecipient = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == transferClient.TransactionRecipient);
                    if (cardRecipient == null) return null;

                    string FromInfoCard = cardSender.NameCard + " •• " + cardSender.NumberCard.Substring(cardSender.NumberCard.Length - 4);
                    string ToInfoCard = cardRecipient.NameCard + " •• " + cardRecipient.NumberCard.Substring(cardRecipient.NumberCard.Length - 4);

                    TransactionValuteBetween transactionValuteBetween = new()
                    {
                        FromCardName = FromInfoCard,
                        ToCardName = ToInfoCard,
                        CountValute = listCurrentle.Arrival,
                        ValuteType = listCurrentle.ValuteTransactions
                    };
                    transactions.TypeTransaction = listCurrentle.CodeOperation;
                    transactions.NameTransaction = listCurrentle.NameOperation;
                    transactions.TransactionId = transferClient.TransactionsID;
                    transactions.DateTime = listCurrentle.ArrivalDate.Value;
                    transactions.TransactionValuteBetween = transactionValuteBetween;
                    return transactions;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                using ApplicationContext _context = new();
                CurrencyTransfer? currencyTransaction = await _context.CurrencyTransfers.Include(x => x.Transactions).FirstOrDefaultAsync(x => x.TransactionsID == transferClient.CurrencyTransferID);
                if (currencyTransaction == null) return null;

                TransactionValute transactionValute = new()
                {
                     BuySale = currencyTransaction.BuySaleValute,
                     ValuteA = currencyTransaction.ValuteA,
                     ValuteB = currencyTransaction.ValuteB,
                     CountValuteA = currencyTransaction.Transactions.Expenses.Value, // Значения для транзакции взяли из currencyTransaction и его транзакции перевода валюты
                     CountValuteB = currencyTransaction.Transactions.Arrival.Value
                };

                // Проверяем, это транзакция на входящий или исходящие валюты ?
                if (transferClient.Transactions.All(x => x.CodeOperation == operationIncomingTransfer || x.CodeOperation == operationOutgoingTransfer) == true)
                {
                    TransactionPaymentServices transactionPaymentServices = new()
                    {
                        TransactionValute = transactionValute
                    };
                    if (transferClient.UserRecipient == userID) // Наш человек получатель?
                    {
                        Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationIncomingTransfer); // Ищем входящий перевод
                        if (listCurrentle == null) return null;
                        // Заполняем модель оплаты 
                        transactionPaymentServices.NameClient = transferClient.NameSender;
                        transactionPaymentServices.CountMoney = currencyTransaction.Transactions.Arrival.Value;
                        transactionPaymentServices.ValuteType = currencyTransaction.ValuteB;
                        // Заполняем модель трансфера

                        transactions.TypeTransaction = operationIncomingCurrencyTransfer; // Новый тип операции расширенный для мобильных устройств
                        transactions.NameTransaction = listCurrentle.NameOperation;
                        transactions.TransactionId = transferClient.TransactionsID;
                        transactions.DateTime = listCurrentle.ArrivalDate.Value; // Дата доставки транзакции
                        transactions.TransactionPaymentServices = transactionPaymentServices;
                        GC.Collect();
                        return transactions;
                    }
                    else // Наш человек отправитель!
                    {
                        Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationOutgoingTransfer); // Ищем исходящий перевод
                        if (listCurrentle == null) return null;
                        // Заполняем модель оплаты 
                        transactionPaymentServices.NameClient = transferClient.NameRecipient;
                        transactionPaymentServices.CountMoney = currencyTransaction.Transactions.Expenses.Value;
                        transactionPaymentServices.ValuteType = currencyTransaction.ValuteA;
                        // Заполняем модель трансфераы
                        // Заполняем транзакцию 

                        transactions.TypeTransaction = operationOutgoingCurrencyTransfer;// Новый тип операции расширенный для мобильных устройств
                        transactions.NameTransaction = listCurrentle.NameOperation;
                        transactions.TransactionId = transferClient.TransactionsID;
                        transactions.DateTime = listCurrentle.ExpensesDate.Value; // Дата принятия валюты
                        transactions.TransactionPaymentServices = transactionPaymentServices; // Добавляем транзакцию оплаты
                        GC.Collect();
                        return transactions;

                    }
                }
                else if (transferClient.Transactions.All(x => x.CodeOperation == operationBetweenCards) == true)
                {
                    Transactions? listCurrentle = transferClient.Transactions.FirstOrDefault(x => x.CodeOperation == operationBetweenCards); // Ищем перевод между счетами
                    if (listCurrentle == null) return null;
                    var cardSender = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == transferClient.TransactionSender);
                    if (cardSender == null) return null;
                    var cardRecipient = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == transferClient.TransactionRecipient);
                    if (cardRecipient == null) return null;

                    string FromInfoCard = cardSender.NameCard + " •• " + cardSender.NumberCard.Substring(cardSender.NumberCard.Length - 4);
                    string ToInfoCard = cardRecipient.NameCard + " •• " + cardRecipient.NumberCard.Substring(cardRecipient.NumberCard.Length - 4);
                    TransactionValuteBetween transactionValuteBetween = new()
                    {
                        FromCardName = FromInfoCard,
                        ToCardName = ToInfoCard,
                        TransactionValute = transactionValute
                    };
                    transactions.TypeTransaction = operationBetweenCurrencyCards;// Новый тип операции расширенный для мобильных устройств
                    transactions.NameTransaction = listCurrentle.NameOperation;
                    transactions.TransactionId = transferClient.TransactionsID;
                    transactions.DateTime = listCurrentle.ArrivalDate.Value;
                    transactions.TransactionValuteBetween = transactionValuteBetween;
                    return transactions;
                }
                else
                {
                    return null;
                }

            }
        }
        public async Task<List<AllTransactions>?> GetAllCardsTransactions(DateTime dateIN, DateTime dateFrom, Guid userID)
        {
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

