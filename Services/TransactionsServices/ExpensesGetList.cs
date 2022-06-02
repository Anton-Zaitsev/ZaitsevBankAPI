using Microsoft.EntityFrameworkCore;
using System;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Models.TransactionsModels;
using static ZaitsevBankAPI.FunctionBank.Operation;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class ExpensesGetList
    {
        private readonly int operationOutgoingTransfer = (int)OperationNumber.OutgoingTransfer; //Исходящий
        private readonly int operationPaymentCredit = (int)OperationNumber.PaymentCredit; // Погащение кредита
        private readonly int operationPayment = (int)OperationNumber.PaymentServices;
        private readonly List<string> allvalute = new CreditCard().CardValute;
        public async Task<List<Expenses>?> ExpensesAsync(string userID)
        {
            Guid user = Guid.Parse(userID);
            DateTime now = DateTime.Now;
            DateTime first = new (now.Year, now.Month, 1);
            DateTime last = new DateTime(now.Year, now.Month + 1, 1).AddDays(-1);

            var taskPayment = GetPaymentServicesAsync(user, first, last);
            var taskCurrency = GetCurrencyAsync(user, first, last);
            var taskPaymentCredit = GetPaymentCreditAsync(user, first, last);
            var taskValueOutgoingTransfer = GetValueOutgoingTransferAsync(user, first, last);

            await Task.WhenAll(taskPayment, taskCurrency, taskPaymentCredit, taskValueOutgoingTransfer);
            
            if (taskPayment.Result != null || taskCurrency.Result != null || taskPaymentCredit.Result != null || taskValueOutgoingTransfer.Result != null)
            {
                List<Expenses> list = new();
                List<double> listSumm = new();

                foreach (string valute in allvalute)
                {
                    list.Add(new Expenses { valuteType = valute });
                    listSumm.Add(0);
                }
                if (taskPayment.Result != null)
                {
                    foreach ((double, string) item in taskPayment.Result)
                    {
                        int indexType = list.FindIndex(x => x.valuteType == item.Item2);
                        if (indexType != -1)
                        {
                            list[indexType].other = list[indexType].other + item.Item1;
                            listSumm[indexType] = listSumm[indexType] + item.Item1;
                        }
                    }
                }
                if (taskCurrency.Result != null)
                {
                    foreach ((double, string, bool) item in taskCurrency.Result)
                    {
                        int indexType = list.FindIndex(x => x.valuteType == item.Item2);
                        if (indexType != -1)
                        {
                            if (item.Item3)
                            {
                                list[indexType].buyValute = list[indexType].buyValute + item.Item1;
                            }
                            else
                            {
                                list[indexType].saleValute = list[indexType].saleValute +  item.Item1;
                            }
                            listSumm[indexType] = listSumm[indexType] + item.Item1;
                        }
                    }
                }
                if (taskPaymentCredit.Result != null)
                {
                    int indexType = list.FindIndex(x => x.valuteType == taskPaymentCredit.Result.Value.Item2);
                    if (indexType != -1)
                    {
                        double value = taskPaymentCredit.Result.Value.Item1;
                        list[indexType].transferCredit = list[indexType].transferCredit + value;
                        listSumm[indexType] = listSumm[indexType] + value;
                    }
                }
                if (taskValueOutgoingTransfer.Result != null)
                {
                    foreach ((double, string) item in taskValueOutgoingTransfer.Result)
                    {
                        int indexType = list.FindIndex(x => x.valuteType == item.Item2);
                        if (indexType != -1)
                        {
                            double summ = item.Item1;
                            list[indexType].transfers = list[indexType].transfers + summ;
                            double lol = list[indexType].transfers;
                            listSumm[indexType] = listSumm[indexType] + summ;
                        }
                    }
                }
                for (int index = 0; index < list.Count; index++)
                {
                    double summAll = listSumm[index];
                    if (summAll > 0)
                    {
                        list[index].summ = summAll;
                        list[index].transfers = list[index].transfers == 0 ? 0 : Math.Round(list[index].transfers / summAll, 4);
                        list[index].buyValute = list[index].buyValute == 0 ? 0 : Math.Round(list[index].buyValute / summAll, 4);
                        list[index].saleValute = list[index].saleValute == 0 ? 0 : Math.Round(list[index].saleValute / summAll, 4);
                        list[index].transferCredit = list[index].transferCredit == 0 ? 0 : Math.Round(list[index].transferCredit / summAll, 4);
                        list[index].other = list[index].other == 0 ? 0 : Math.Round(list[index].other / summAll, 4);
                    }
                }
                list.RemoveAll(x => x.summ == 0);
                GC.Collect();;
                return list.Count > 0 ? list.OrderByDescending(x => x.valuteType.Contains("RUB")).ToList() : null;
            }
            else
            {
                return null;
            }

        }
        private async Task<List<(double, string)>?> GetPaymentServicesAsync(Guid userID, DateTime In, DateTime Out)
        {
            using ApplicationContext _context = new();
            var transaction = await _context.PaymentServices.Include(x => x.Transactions).Where(x => x.UserID == userID && x.Transactions.ArrivalDate >= In && x.Transactions.ArrivalDate <= Out && x.Transactions.CodeOperation == operationPayment).ToListAsync();
            if (transaction.Count == 0) return null;
            List<(double, string)> listValues = new();
            foreach (var valute in allvalute)
            {
                listValues.Add((0, valute));
            }
            foreach (PaymentServices item in transaction)
            {
                int index = listValues.FindIndex(x => x.Item2 == item.Transactions.ValuteTransactions);
                if (index != -1)
                {
                    listValues[index] = (listValues[index].Item1 + item.Transactions.Expenses.Value, item.Transactions.ValuteTransactions);
                }
                
            }
            listValues.RemoveAll(x => x.Item1 == 0);
            GC.Collect();
            return listValues.Count > 0 ? listValues : null;
        }
        private async Task<List<(double, string, bool)>?> GetCurrencyAsync(Guid userID, DateTime In, DateTime Out)
        {
            using ApplicationContext _context = new();
            var transaction = await _context.CurrencyTransfers.Include(x => x.Transactions).Where(x => x.UserID == userID && x.Transactions.ArrivalDate >= In && x.Transactions.ArrivalDate <= Out).ToListAsync();
            if (transaction.Count == 0) return null;
            List<(double, string, bool)> listValues = new();
            foreach (var valute in allvalute)
            {
                listValues.Add((0, valute,false));
            }
            foreach (CurrencyTransfer item in transaction)
            {
                if (item.Transactions.ArrivalDate >= In && item.Transactions.ArrivalDate <= Out)
                {
                    var cardCheck = await _context.Cards.Where(x => x.TransactionCard == item.TransactionCardFrom || x.TransactionCard == item.TransactionCardTo).ToListAsync();
                    if (cardCheck.Count > 1)
                    {
                        if (cardCheck[0].UserID == cardCheck[1].UserID)
                        {
                            int index = listValues.FindIndex(x => x.Item2 == item.ValuteA);
                            if (index != -1)
                            {
                                listValues[index] = (listValues[index].Item1 + item.Transactions.Expenses.Value, item.ValuteA, item.BuySaleValute);
                            }
                        }
                    }
                }
            }
            listValues.RemoveAll(x => x.Item1 == 0);
            GC.Collect();
            return listValues.Count > 0 ? listValues : null;
        }

        private async Task<(double, string)?> GetPaymentCreditAsync(Guid userID, DateTime In, DateTime Out)
        {
            using ApplicationContext _context = new();
            var transactionList = await _context.Credits.Include(x => x.Transactions).Where(x => x.UserID == userID && x.Transactions.Any(y => y.ArrivalDate >= In && y.ArrivalDate <= Out && y.CodeOperation == operationPaymentCredit)).ToListAsync();
            if (transactionList.Count == 0) return null;
            double summ = 0;
            foreach (Credits item in transactionList)
            {
                foreach (Transactions transactions in item.Transactions)
                {
                    if (transactions.CodeOperation == operationPaymentCredit && transactions.ArrivalDate >= In && transactions.ArrivalDate <= Out)
                    {
                        summ += transactions.Arrival.Value;
                    }
                }
            }
            GC.Collect();
            return summ > 0 ? (summ, "RUB") : null;
        }
        private async Task<List<(double,string)>?> GetValueOutgoingTransferAsync(Guid userID, DateTime In, DateTime Out)
        {
            using ApplicationContext _context = new();
            var transaction = await _context.TransferClient.Include(x => x.Transactions).Where(x => x.UserSender == userID && x.Transactions.Any(y => y.ArrivalDate >= In && y.ArrivalDate <= Out)).ToListAsync();
            if (transaction.Count == 0) return null;
            List<(double, string)> listValues = new();
            foreach (var valute in allvalute)
            {
                listValues.Add((0, valute));
            }
            foreach (TransferClient item in transaction)
            {
                var data = item.Transactions.FirstOrDefault(x => x.ArrivalDate >= In && x.ArrivalDate <= Out && x.CodeOperation == operationOutgoingTransfer);
                if (data != null)
                {
                    int index = listValues.FindIndex(x => x.Item2 == data.ValuteTransactions);
                    if (index != -1)
                    {
                        listValues[index] = (listValues[index].Item1 + data.Expenses.Value, data.ValuteTransactions);
                    }
                }
            }
            listValues.RemoveAll(x => x.Item1 == 0);
            GC.Collect();
            return listValues.Count > 0 ? listValues : null;
        }
    }
}
