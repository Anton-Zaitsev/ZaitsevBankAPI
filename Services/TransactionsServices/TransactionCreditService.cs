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

        private readonly static double stavkaCB = 14; // Ставка ЦБ
        private readonly double stavkaZaitsevBank = stavkaCB + 7; // Ставка ZaitsevBank
        private readonly float summMin = 60000; // Минимальная сумма кредита
        private readonly float summMax = 4500000;  // Максимальная сумма кредита
        private readonly float summMinStavka = 2000000; // Сумма при которой уменьшиться процентная ставка
        private readonly double MinStavkaZaitsevBank = stavkaCB + 3.5; // Минимальная ставка
        private readonly int yearMin = 1; // Минимальное количество которое берет кредит
        private readonly int yearMax = 7; // Максимальное количество, которое берет кредит
        private readonly string ValuteCredit = "RUB"; // Кредитная валюта

        // ----------------------------- ФУНКЦИИ И МЕТОДЫ РАСЧЕТА ------------------------------

        // Функция расчета платежа по кредиту где 
        private double GetPay (double S, double i, int n)
        {
            double result = S * (i + i / (Math.Pow(1 + i, n) - 1));
            return result;
        }

        // Функция расчета суммы процентов а платеже
        private double GetPercentPay(double Sn, double i)
        {
            return Sn * i;
        }

        // Функция расчета суммы остатка по кредиту
        private double GetLast(double Sn, double P, double P1)
        {
            return Sn - (P - P1);
        }


        public CreditCheck? creditCheck(string count,string countMonthly,float? stavka = null)
        {
            double value;
            int month;
            if (double.TryParse(count, out value))
            {
                if (int.TryParse(countMonthly, out month))
                {
                    if (value >= summMin && value <= summMax && month >= yearMin && month <= yearMax)
                    {

                        month *= 12; // Год переведим в месяца;

                        CreditCheck credit = new();
                        credit.monthCredit = month;
                        credit.summCredit = value;
                        credit.persent = stavka == null ? (value >= summMinStavka ? MinStavkaZaitsevBank : stavkaZaitsevBank) : stavka.Value;

                        // S - остаточная стоимость кредита
                        // i - 1/12 годовой процентной ставки
                        // n - срок выплат
                        // P - сумма платежа
                        // P1 - сумма процентов в платеже 
                        double P = 0, P1 = 0, Sn = value;
                        List<PaymentsCredit> paymentsCredits = new();
                        double monthPersent = (credit.persent / 12) / 100;
                        for (int i = 0; i < month; i++)
                        {
                            P = Math.Round(GetPay(Sn, monthPersent, month - i), 2);
                            P1 = Math.Round(GetPercentPay(Sn, monthPersent), 2);
                            Sn = Math.Round(GetLast(Sn, P, P1));
                            PaymentsCredit p = new PaymentsCredit { 
                                month = i + 1, 
                                pay = i == month - 1 ? paymentsCredits[i - 1].lastSumm : P, 
                                percents = P1, 
                                lastSumm = Sn
                            };
                            paymentsCredits.Add(p);
                            credit.overPayment += (float)P1;
                        }
                        credit.monthlyPayment = (float)paymentsCredits[0].pay;
                        credit.paymentsCredits = paymentsCredits;
                        return credit;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddMoneyCredit(string transactionCard, string creditID)
        {
            int typeOperationPayment = (int)Operation.OperationNumber.CreditPaymentExpected;
            Guid TransactionCreditID = Guid.Parse(creditID);
            // Проверяем есть ли у нас кредит и проверяем, нужно ли нам платить в этом месяце
            var credit = await _context.Credits.Include(y => y.Transactions).FirstOrDefaultAsync(x => x.TransactionsID == TransactionCreditID 
            && x.Transactions.Any(x => x.CodeOperation == typeOperationPayment && x.ExpensesDate.Value.Month == DateTime.Today.Month && x.ExpensesDate.Value.Year == DateTime.Today.Year));
            if (credit == null) return false;

            var creditModel = creditCheck(credit.CreditSumm.ToString(), (credit.Period / 12).ToString(), credit.Rate);
            if (creditModel == null) return false;

            var Collection = credit.Transactions.FirstOrDefault(x => x.CodeOperation == typeOperationPayment && x.ExpensesDate.Value.Month == DateTime.Today.Month && x.ExpensesDate.Value.Year == DateTime.Today.Year);
            if (Collection == null) return false;
            var summPay = creditModel.paymentsCredits.FirstOrDefault(x => x.lastSumm == Collection.Expenses.Value);
            if (summPay == null) return false;

            Guid TransactionCard = Guid.Parse(transactionCard);
            var card = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == TransactionCard);
            if (card == null) return false;
            if (card.MoneyCard < summPay.pay) return false;

            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionsID == Collection.TransactionsID && x.CodeOperation == typeOperationPayment);
            if (transaction == null) return false;

            card.MoneyCard -= summPay.pay;
            Operation.OperationNumber paymentCredit = Operation.OperationNumber.PaymentCredit;

            transaction.CodeOperation = (int)paymentCredit;
            transaction.NameOperation = Operation.getNameOperation(paymentCredit);
            transaction.Arrival = summPay.pay;
            transaction.ArrivalDate = DateTime.Now;

            _context.Cards.Update(card);
            _context.Transactions.Update(transaction);

            if (transaction.Expenses == 0)
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
                await _context.Transactions.AddAsync(TransactionsENDMoney);
            }
            await _context.SaveChangesAsync();
            GC.Collect();
            return true;
        }
        public async Task<bool> ApplyCredit(string count, string year, string transactionCard)
        {
            var credit = creditCheck(count, year);
            if (credit == null) return false;

            Guid TransactionCard = Guid.Parse(transactionCard);
            var card = await _context.Cards.FirstOrDefaultAsync(x => x.TransactionCard == TransactionCard);
            if (card == null) return false;
            if (card.TypeMoney != ValuteCredit) return false;

            Operation.OperationNumber takeCredit = Operation.OperationNumber.TakeCredit;
            Operation.OperationNumber awaitCredit = Operation.OperationNumber.CreditPaymentExpected;
            string operation_await_text = Operation.getNameOperation(awaitCredit);

            List<Transactions> list = new();
            Transactions transactions = new()
            {
                TransactionsID = Guid.NewGuid(),
                CodeOperation = (int)takeCredit,
                NameOperation = Operation.getNameOperation(takeCredit),
                ArrivalDate = DateTime.Now,
                ExpensesDate = DateTime.Today.AddMonths(credit.monthCredit),
                Arrival = credit.summCredit,
                Expenses = 0,
                ValuteTransactions = ValuteCredit 
            };
            list.Add(transactions);
            foreach (PaymentsCredit creditMonth in credit.paymentsCredits)
            {
                Transactions transactionsCredit = new()
                {
                    TransactionsID = Guid.NewGuid(),
                    CodeOperation = (int)awaitCredit,
                    NameOperation = operation_await_text,
                    ExpensesDate = DateTime.Today.AddMonths(creditMonth.month),
                    Arrival = 0,
                    Expenses = (double)creditMonth.lastSumm,
                    ValuteTransactions = ValuteCredit
                };
                list.Add(transactionsCredit);
            }
            Credits credits = new()
            {
                UserID = card.UserID,
                NumberDocument = new Random().Next(0, 99999),
                CreditSumm = Math.Round(credit.summCredit),
                Rate = (float)credit.persent,
                Period = credit.monthCredit,
                TransactionCard = TransactionCard,
                Transactions = list
            };
            card.MoneyCard = Math.Round(card.MoneyCard + credit.summCredit, 2);
            _context.Cards.Update(card);
            await _context.Credits.AddAsync(credits);
            await _context.SaveChangesAsync();
            GC.Collect();
            return true;

        }
                      
    }
}
