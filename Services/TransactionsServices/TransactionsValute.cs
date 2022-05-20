using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class TransactionsValute
    {
        private readonly ApplicationContext _context;
        public TransactionsValute()
        {
            _context = new();
        }

        public async Task<bool> ValuteBuySale(string CardA, string CardB, string summ, bool BuySale)
        {
            double value;
            if (double.TryParse(summ, out value))
            {
                Guid TransactionA = Guid.Parse(CardA);
                Guid TransactionB = Guid.Parse(CardB);

                CardService cardService = new();

                var cardA = await cardService.GetCardClient(TransactionA);
                if (cardA == null) return false;
                var cardB = await cardService.GetCardClient(TransactionB);
                if (cardB == null) return false;

                if (cardA.UserID != cardB.UserID || cardA.TypeMoney == cardB.TypeMoney) return false;

                var count = await CurrencyTransferTransaction(cardA.UserID, TransactionA, cardA.TypeMoney, cardB.TypeMoney, value, BuySale);
                if (count == null) return false;

                cardA.MoneyCard -= value;
                cardB.MoneyCard += count.Transactions.Arrival.Value;

                CreditCard creditCard = new();
                cardA.MoneyCard = Math.Round(cardA.MoneyCard, creditCard.isElectronValute(cardA.TypeMoney) ? 6 : 2);
                cardB.MoneyCard = Math.Round(cardB.MoneyCard, creditCard.isElectronValute(cardB.TypeMoney) ? 6 : 2);

                _context.Cards.UpdateRange(cardA, cardB);
                await _context.SaveChangesAsync(); // Сохраняем изменения для перевода
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<CurrencyTransfer?> CurrencyTransferTransaction(Guid UserID, Guid TransactionCard, string ValuteA, string ValuteB, double countValute, bool BuySale = true)
        {
            ExchangesService exchangesService = new();
            var transformValute = await exchangesService.GetActualCurseRUB(ValuteA, ValuteB, BuySale);
            if (transformValute == null) return null;

            var dataExchange = await exchangesService.ValuteATOValuteB(ValuteA, ValuteB, BuySale, countValute); // Только покупка
            if (dataExchange == null) return null;

            Operation.OperationNumber operationNumber = Operation.OperationNumber.CurrencyTransfer;

            Guid IDTransaction = Guid.NewGuid();

            Transactions transactions = new()
            {
                TransactionsID = IDTransaction,
                CodeOperation = (int)operationNumber,
                NameOperation = Operation.getNameOperation(operationNumber),
                Arrival = dataExchange.ValuteConvert, // Приход валюты B
                Expenses = countValute, // Расход Валюты А
                ArrivalDate = DateTime.Now,
                ExpensesDate = DateTime.Now
            };

            CurrencyTransfer currencyTransfer = new()
            {
                TransactionsID = IDTransaction,
                NumberDocument = new Random().Next(0, 99999),
                UserID = UserID,
                TransactionCard = TransactionCard,
                ValuteA = ValuteA,
                ValuteB = ValuteB,
                ActualCurseRub = transformValute.Value,
                Transactions = transactions,
                BuySaleValute = BuySale
            };

            await _context.CurrencyTransfers.AddAsync(currencyTransfer); // Добавляем транзакцию перевода валют
            await _context.SaveChangesAsync(); // Сохраняем изменения для перевода

            return currencyTransfer;
        }
    }
}
