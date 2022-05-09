using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class TransactionsService
    {
        private readonly ApplicationContext _context;
        public TransactionsService()
        {
            _context = new();
        }

        public async Task CreateCard(Guid UserID, Guid DebitCard, string CardInfo)
        {
            Operation.OperationNumber activationCard = Operation.OperationNumber.ActivationCard;

            Guid idTransaction = Guid.NewGuid();
            Transactions activateTransaction = new()
            {
                TransactionsID = idTransaction,
                CodeOperation = (int)activationCard,
                NameOperation = Operation.getNameOperation(activationCard),
                ArrivalDate = DateTime.Now,
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

        public async Task<bool> GetTransaction(string UserID)
        {
            Guid userID = Guid.Parse(UserID);
            var data = await _context.CurrencyTransfers.Include(x => x.Transactions).Where(x => x.UserID == userID).ToListAsync();
            foreach (var item in data)
            {
                var dataID = item.Transactions.TransactionsID;
                string datadsfsdf = dataID.ToString();
            }
            return data.Count > 0;
        }
        public async Task<bool> TransferToClient(string transactionSender, string transactionRecipient, string summ)
        {

            double value;
            if (double.TryParse(summ, out value))
            {
                Guid TransactionSender = Guid.Parse(transactionSender);
                Guid TransactionRecipient = Guid.Parse(transactionRecipient);
                CardService CardService = new();
                var cardSender = await CardService.GetCardClient(TransactionSender);
                if (cardSender == null) return false;
                if (cardSender.ClosedCard == false && cardSender.MoneyCard - value >= 0)
                {
                    var cardRecipient = await CardService.GetCardClient(TransactionRecipient);
                    if (cardRecipient == null) return false;
                    if (cardRecipient.ClosedCard == true) return false;

                    UserService userService = new();
                    var userSender = await userService.GetUserData(cardSender.UserID.ToString());
                    if (userSender == null) return false;

                    var userRecipient = await userService.GetUserData(cardRecipient.UserID.ToString());
                    if (userRecipient == null) return false;

                    CreditCard creditCard = new();
                    if (userSender.UserID != userRecipient.UserID) // Проверяем, не одинаковые ли это пользователи (перевод между картами)
                    {
                        Operation.OperationNumber sender = Operation.OperationNumber.OutgoingTransfer;
                        Operation.OperationNumber recipient = Operation.OperationNumber.IncomingTransfer;


                        string middlenameSender = userSender.MiddleName == null ? "" : $"{userSender.MiddleName} ";
                        string lastNameSender = userSender.MiddleName == null ? userSender.LastName : $"{userSender.LastName.First()}.";

                        string middlenameRecipient = userRecipient.MiddleName == null ? "" : $"{userRecipient.MiddleName} ";
                        string lastNameRecipient = userRecipient.MiddleName == null ? userRecipient.LastName : $"{userRecipient.LastName.First()}.";


                        Transactions transactionsSender = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)sender,
                            NameOperation = Operation.getNameOperation(sender),
                            Arrival = 0,
                            Expenses = value,
                            ArrivalDate = DateTime.Now

                        };
                        Transactions transactionsRecipient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)recipient,
                            NameOperation = Operation.getNameOperation(recipient),
                            Arrival = value,
                            Expenses = 0,
                            ExpensesDate = DateTime.Now
                        };

                        // Делаем проверку на валюту, если она разная сделаем манипуляцию и перевод
                        if (cardSender.TypeMoney != cardRecipient.TypeMoney)
                        {
                            var countValute = await CurrencyTransferTransaction(userSender.UserID, TransactionSender, cardSender.TypeMoney, cardRecipient.TypeMoney, value);
                            if (countValute == null) return false;

                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard = (double)(cardRecipient.MoneyCard + countValute);

                            // Делаем транзакцию для перевода валюты
                        }
                        else
                        {
                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard += value;
                        }

                        cardSender.MoneyCard = Math.Round(cardSender.MoneyCard, creditCard.isElectronValute(cardSender.TypeMoney) ? 6 : 2);
                        cardRecipient.MoneyCard = Math.Round(cardRecipient.MoneyCard, creditCard.isElectronValute(cardRecipient.TypeMoney) ? 6 : 2);
                        _context.Cards.UpdateRange(cardSender, cardRecipient);
                        await _context.SaveChangesAsync(); // Сохраняем изменения для перевода карт

                        transactionsSender.ExpensesDate = DateTime.Now;
                        transactionsRecipient.ArrivalDate = DateTime.Now;

                        List<Transactions> transactions = new();
                        transactions.Add(transactionsSender);
                        transactions.Add(transactionsRecipient);

                        TransferClient transferClient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            UserSender = userSender.UserID,
                            UserRecipient = userRecipient.UserID,
                            Transactions = transactions,
                            NumberDocument = new Random().Next(0, 99999),
                            NameSender = userSender.FirstName + " " + middlenameSender + lastNameSender,
                            NameRecipient = userRecipient.FirstName + " " + middlenameRecipient + lastNameRecipient,
                            TransactionRecipient = TransactionRecipient,
                            TransactionSender = TransactionSender
                        };

                        await _context.TransferClient.AddAsync(transferClient);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Operation.OperationNumber between = Operation.OperationNumber.BetweenMyCards;

                        string nameSender = userSender.MiddleName == null ? "" : $"{userSender.MiddleName} ";
                        string lastNameSender = userSender.MiddleName == null ? userSender.LastName : $"{userSender.LastName.First()}.";

                        Transactions transactionsBetween = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)between,
                            NameOperation = Operation.getNameOperation(between),
                            Arrival = 0,
                            Expenses = 0
                        };

                        // Делаем проверку на валюту, если она разная сделаем манипуляцию и перевод
                        if (cardSender.TypeMoney != cardRecipient.TypeMoney)
                        {
                            var countValute = await CurrencyTransferTransaction(userSender.UserID, TransactionSender, cardSender.TypeMoney, cardRecipient.TypeMoney, value);
                            if (countValute == null) return false;

                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard = (double)(cardRecipient.MoneyCard + countValute);

                            // Делаем транзакцию для перевода валюты
                        }
                        else
                        {
                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard += value;
                        }
                        cardSender.MoneyCard = Math.Round(cardSender.MoneyCard, creditCard.isElectronValute(cardSender.TypeMoney) ? 6 : 2);
                        cardRecipient.MoneyCard = Math.Round(cardRecipient.MoneyCard, creditCard.isElectronValute(cardRecipient.TypeMoney) ? 6 : 2);
                        _context.Cards.UpdateRange(cardSender, cardRecipient);
                        await _context.SaveChangesAsync(); // Сохраняем изменения для перевода карт

                        transactionsBetween.ExpensesDate = DateTime.Now;
                        transactionsBetween.ArrivalDate = DateTime.Now;

                        List<Transactions> transactions = new();
                        transactions.Add(transactionsBetween);

                        TransferClient transferClient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            UserSender = userSender.UserID,
                            UserRecipient = userRecipient.UserID,
                            Transactions = transactions,
                            NumberDocument = new Random().Next(0, 99999),
                            NameSender = userSender.FirstName + " " + nameSender + lastNameSender,
                            NameRecipient = userSender.FirstName + " " + nameSender + lastNameSender,
                            TransactionRecipient = TransactionRecipient,
                            TransactionSender = TransactionSender
                        };
                        await _context.TransferClient.AddAsync(transferClient);
                        await _context.SaveChangesAsync();
                    }
                    GC.Collect();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ValuteBuySale(string CardA, string CardB, string summ,bool BuySale)
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
                cardB.MoneyCard = (double)(cardB.MoneyCard + count);

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
        private async Task<double?> CurrencyTransferTransaction(Guid UserID, Guid TransactionCard, string ValuteA, string ValuteB, double countValute, bool BuySale = true)
        {
            ExchangesService exchangesService = new();
            var transformValute = await exchangesService.GetActualCurseRUB(ValuteA, ValuteB, BuySale);
            if (transformValute == null) return null;

            var dataExchange = await exchangesService.ValuteATOValuteB(ValuteA, ValuteB, BuySale, countValute); // Только покупка
            if (dataExchange == null) return null;

            Operation.OperationNumber operationNumber = Operation.OperationNumber.CurrencyTransfer;

            Guid idTransaction = Guid.NewGuid();

            Transactions transactions = new()
            {
                TransactionsID = idTransaction,
                CodeOperation = (int)operationNumber,
                NameOperation = Operation.getNameOperation(operationNumber),
                Arrival = dataExchange.ValuteConvert,
                Expenses = countValute,
                ArrivalDate = DateTime.Now,
                ExpensesDate = DateTime.Now
            };

            CurrencyTransfer currencyTransfer = new()
            {
                TransactionsID = idTransaction,
                NumberDocument = new Random().Next(0, 99999),
                UserID = UserID,
                TransactionCard = TransactionCard,
                ValuteA = ValuteA,
                ValuteB = ValuteB,
                ActualCurse = (double)transformValute,
                CountValute = countValute,
                Transactions = transactions
            };
            await _context.CurrencyTransfers.AddAsync(currencyTransfer);
            await _context.SaveChangesAsync();

            return dataExchange.ValuteConvert;
        }
    }
}
