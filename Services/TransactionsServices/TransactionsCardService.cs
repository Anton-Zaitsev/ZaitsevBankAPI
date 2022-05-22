using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class TransactionsCardService
    {
        private readonly ApplicationContext _context;
        public TransactionsCardService()
        {
            _context = new();
        }
        
        public async Task CreateCard(Guid UserID, Guid DebitCard, string CardInfo,string ValuteTransactions)
        {
            Operation.OperationNumber activationCard = Operation.OperationNumber.ActivationCard;

            Guid idTransaction = Guid.NewGuid();
            Transactions activateTransaction = new()
            {
                TransactionsID = idTransaction,
                CodeOperation = (int)activationCard,
                NameOperation = Operation.getNameOperation(activationCard),
                ArrivalDate = DateTime.Now,
                ValuteTransactions = ValuteTransactions
            };
            PaymentServices activationServices = new()
            {
                TransactionsID = idTransaction,
                NumberDocument = new Random().Next(0, 99999),
                DebitCard = DebitCard,
                NameServices = CardInfo,
                UserID = UserID,
                Transactions = activateTransaction
            };
            await _context.PaymentServices.AddAsync(activationServices);
            await _context.SaveChangesAsync();
        }
        private PaymentServices ClosedCard(Guid UserID, Guid DebitCard, string CardInfo, string ValuteTransactions)
        {
            Operation.OperationNumber deActivationCard = Operation.OperationNumber.DeActivationCard;

            Guid idTransaction = Guid.NewGuid();
            Transactions DeActivateTransaction = new()
            {
                TransactionsID = idTransaction,
                CodeOperation = (int)deActivationCard,
                NameOperation = Operation.getNameOperation(deActivationCard),
                ArrivalDate = DateTime.Now,
                ValuteTransactions = ValuteTransactions
            };
            PaymentServices DeactivationServices = new()
            {
                TransactionsID = idTransaction,
                NumberDocument = new Random().Next(0, 99999),
                DebitCard = DebitCard,
                NameServices = CardInfo,
                UserID = UserID,
                Transactions = DeActivateTransaction
            };
            return DeactivationServices;
        }

        public async Task ClosedCards()
        {
            var allCards = await _context.Cards.ToListAsync();
            if (allCards.Count == 0) return;
            DateTime time = DateTime.Now;
            bool closedOneCard = false;
            List<PaymentServices> paymentServices = new();

            Parallel.ForEach(allCards, (line, state, index) =>
            {
                if (allCards[(int)index].DataClosedCard <= time) // Если дейсвительность карты меньше или равно текущему времени, то закрываем карту!
                {
                    allCards[(int)index].ClosedCard = true;
                    string infoCard = allCards[(int)index].NameCard + " •• " + allCards[(int)index].NumberCard.Substring(allCards[(int)index].NumberCard.Length - 4);
                    paymentServices.Add(
                        ClosedCard(allCards[(int)index].UserID, allCards[(int)index].TransactionCard, infoCard, allCards[(int)index].TypeMoney)
                        );
                    closedOneCard = true;
                }
            });

            if (closedOneCard)
            {
                _context.Cards.UpdateRange(allCards);
                await _context.PaymentServices.AddRangeAsync(paymentServices);
                await _context.SaveChangesAsync();
                GC.Collect();
            }
            else
            {
                GC.Collect();
                return;
            }
        }
    }
}
