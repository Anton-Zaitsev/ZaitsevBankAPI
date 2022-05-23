using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class TransactionCreditService
    {
        private readonly ApplicationContext _context;
        public TransactionCreditService()
        {
            _context = new();
        }
        private readonly int default_interpreter = 4; // Понижение процентной ставки, если сумма превышает текущую в 4 раза
        private readonly int max_interpreter = 16; // Максимальное количество превышения
        private readonly int Persernt = 7;
        private readonly int Year = 5; // На 5 лет дефолт
        private readonly int SummCreditFromPersent = 1200000;
        private readonly string ValuteCredit = "RUB";

        public CreditCheck? creditCheck(string count)
        {
            double value;
            if (double.TryParse(count, out value))
            {
                if (value < SummCreditFromPersent) return null;

                int interpreter = default_interpreter;
                int year = Year;
                int persent = Persernt;
                while (value > SummCreditFromPersent * interpreter && interpreter <= max_interpreter)
                {
                    year--;
                    persent--;
                    interpreter += default_interpreter;
                }// Если сумма превышает дефолтную, то понижаем процентную ставку
                double procentSumm = (value / 100) * persent;
                double creditSumm = Math.Round(value + procentSumm,2);
                return new CreditCheck
                {
                       CreditSumm = creditSumm,
                       Period = year,
                       Rate = persent
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddMoneyCredit(string count, string transactionCard, string creditID)
        {
            double value;
            if (double.TryParse(count, out value))
            {
                Guid TransactionCreditID = Guid.Parse(creditID);
                var credit = await _context.Credits.Include(y => y.Transactions).FirstOrDefaultAsync(x => x.TransactionsID == TransactionCreditID);
                if (credit == null) return false;

                Guid TransactionCard = Guid.Parse(transactionCard);
                var card = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == TransactionCard);
                if (card == null) return false;
                if (card.MoneyCard < value) return false;

                Operation.OperationNumber paymentCredit = Operation.OperationNumber.PaymentCredit;
                var transactionsAddMoney = credit.Transactions.Where(x => x.CodeOperation == (int)paymentCredit).ToList();

                List<Transactions> list_transactions = new();
                if (transactionsAddMoney.Count == 0)
                {
                    bool payAllCredit = false;
                    if (Math.Round(value,2) >= Math.Round(credit.CreditSumm,2))
                    {
                        value = Math.Round(value - (value - credit.CreditSumm), 2 );// Остаток, от которого надо избавиться    
                        payAllCredit = true;
                    }
                    Transactions TransactionsAddMoney = new()
                    {
                        TransactionsID = Guid.NewGuid(),
                        CodeOperation = (int)paymentCredit,
                        NameOperation = Operation.getNameOperation(paymentCredit),
                        ArrivalDate = DateTime.Now,
                        Arrival = 0,
                        Expenses = value,
                        ValuteTransactions = ValuteCredit,
                        Credits = credit
                    };
                    list_transactions.Add(TransactionsAddMoney);

                    if (payAllCredit)
                    {
                        Operation.OperationNumber RepaymentCredit = Operation.OperationNumber.RepaymentCredit;

                        Transactions TransactionsENDMoney = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)RepaymentCredit,
                            NameOperation = Operation.getNameOperation(RepaymentCredit),
                            ArrivalDate = DateTime.Now,
                            ValuteTransactions = ValuteCredit,
                            Credits = credit
                        };
                        list_transactions.Add(TransactionsENDMoney);
                    }
                }
                else
                {
                    double count_money_paid = 0; // Выплаченная сумма за весь период
                    foreach (Transactions item in transactionsAddMoney)
                    {
                        count_money_paid += item.Expenses.Value; 
                    }
                    bool payAllCredit = false;
 
                    if (Math.Round(value,2) >= Math.Round(credit.CreditSumm - count_money_paid, 2))
                    {
                        value = Math.Round(value - (value - (credit.CreditSumm - count_money_paid) ),2);// Остаток, от которого надо избавиться    
                        payAllCredit = true;
                    }
                    Transactions TransactionsAddMoney = new()
                    {
                        TransactionsID = Guid.NewGuid(),
                        CodeOperation = (int)paymentCredit,
                        NameOperation = Operation.getNameOperation(paymentCredit),
                        ArrivalDate = DateTime.Now,
                        Arrival = 0,
                        Expenses = value,
                        ValuteTransactions = ValuteCredit,
                        Credits = credit
                    };
                    list_transactions.Add(TransactionsAddMoney);

                    if (payAllCredit)
                    {
                        Operation.OperationNumber RepaymentCredit = Operation.OperationNumber.RepaymentCredit;

                        Transactions TransactionsENDMoney = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)RepaymentCredit,
                            NameOperation = Operation.getNameOperation(RepaymentCredit),
                            ArrivalDate = DateTime.Now,
                            ValuteTransactions = ValuteCredit,
                            Credits = credit
                        };
                        list_transactions.Add(TransactionsENDMoney);
                    }
                }
                card.MoneyCard = Math.Round(card.MoneyCard - value, 2);
                if (card.MoneyCard < 0) return false;
                await _context.Transactions.AddRangeAsync(list_transactions);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> ApplyCredit(string count, string transactionCard)
        {
            double value;
            if (double.TryParse(count, out value))
            {
                if (Math.Round(value,2) < SummCreditFromPersent) return false;

                Guid TransactionCard = Guid.Parse(transactionCard);
                var card = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == TransactionCard);
                if (card == null) return false;
                if (card.TypeMoney != ValuteCredit) return false;

                var credit_check =  creditCheck(count);
                if (creditCheck == null) return false;

                Operation.OperationNumber takeCredit = Operation.OperationNumber.TakeCredit;
                Transactions transactions = new()
                {
                    TransactionsID = Guid.NewGuid(),
                    CodeOperation = (int)takeCredit,
                    NameOperation = Operation.getNameOperation(takeCredit),
                    ArrivalDate = DateTime.Now,
                    Arrival = Math.Round(value,2),
                    Expenses = 0,
                    ValuteTransactions = ValuteCredit 
                };
                List<Transactions> list = new();
                list.Add(transactions);

                Credits credits = new()
                {
                    UserID = card.UserID,
                    NumberDocument = new Random().Next(0, 99999),
                    CreditSumm = Math.Round(credit_check.CreditSumm,2),
                    Rate = credit_check.Rate,
                    Period = DateTime.Now.AddYears(credit_check.Period),
                    TransactionCard = TransactionCard,
                    Transactions = list
                };
                card.MoneyCard = Math.Round(card.MoneyCard + value, 2);
                _context.Cards.Update(card);
                await _context.Credits.AddAsync(credits);
                await _context.SaveChangesAsync();
                GC.Collect();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
