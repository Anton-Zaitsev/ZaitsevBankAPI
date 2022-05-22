using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models.TransactionsModels;

namespace ZaitsevBankAPI.Services.TransactionsServices
{
    public class TransactionsTransferService
    {
        private readonly ApplicationContext _context;
        public TransactionsTransferService()
        {
            _context = new();
        }
        public async Task<bool> TransferToClient(string transactionSender, string transactionRecipient, string summ)
        {

            double value;
            if (double.TryParse(summ, out value))
            {
                if (value == 0) return false;
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
                            ArrivalDate = DateTime.Now,
                            ValuteTransactions = cardSender.TypeMoney

                        };
                        Transactions transactionsRecipient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            CodeOperation = (int)recipient,
                            NameOperation = Operation.getNameOperation(recipient),
                            Expenses = 0,
                            ExpensesDate = DateTime.Now,
                            ValuteTransactions = cardRecipient.TypeMoney
                        };

                        TransferClient transferClient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            UserSender = userSender.UserID,
                            UserRecipient = userRecipient.UserID,
                            NumberDocument = new Random().Next(0, 99999),
                            NameSender = userSender.FirstName + " " + middlenameSender + lastNameSender,
                            NameRecipient = userRecipient.FirstName + " " + middlenameRecipient + lastNameRecipient,
                            TransactionRecipient = TransactionRecipient,
                            TransactionSender = TransactionSender
                        };

                        // Делаем проверку на валюту, если она разная сделаем манипуляцию и перевод
                        if (cardSender.TypeMoney != cardRecipient.TypeMoney)
                        {
                            TransactionsValute transactionsValute = new();
                            var countValute = await transactionsValute.CurrencyTransferTransaction(userSender.UserID, TransactionSender, TransactionRecipient, cardSender.TypeMoney, cardRecipient.TypeMoney, value);
                            if (countValute == null) return false;

                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard += countValute.Transactions.Arrival.Value;

                            transactionsRecipient.Arrival = countValute.Transactions.Arrival.Value; // Приход с перевода валюты для карты B
                            transferClient.CurrencyTransferID = countValute.TransactionsID;
                            // Делаем транзакцию для перевода валюты
                        }
                        else
                        {
                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard += value;

                            transactionsSender.Expenses = value; // Расход для карты А
                            transactionsRecipient.Arrival = value; // Приход для карты B
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

                        transferClient.Transactions = transactions;

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
                            NameOperation = Operation.getNameOperation(between)
                        };

                        TransferClient transferClient = new()
                        {
                            TransactionsID = Guid.NewGuid(),
                            UserSender = userSender.UserID,
                            UserRecipient = userRecipient.UserID,
                            NumberDocument = new Random().Next(0, 99999),
                            NameSender = userSender.FirstName + " " + nameSender + lastNameSender,
                            NameRecipient = userSender.FirstName + " " + nameSender + lastNameSender,
                            TransactionRecipient = TransactionRecipient,
                            TransactionSender = TransactionSender
                        };

                        // Делаем проверку на валюту, если она разная сделаем манипуляцию и перевод
                        if (cardSender.TypeMoney != cardRecipient.TypeMoney)
                        {
                            TransactionsValute transactionsValute = new();
                            var countValute = await transactionsValute.CurrencyTransferTransaction(userSender.UserID, TransactionSender, TransactionRecipient, cardSender.TypeMoney, cardRecipient.TypeMoney, value);
                            if (countValute == null) return false;

                            cardSender.MoneyCard -= value;
                            cardRecipient.MoneyCard += countValute.Transactions.Arrival.Value;

                            transferClient.CurrencyTransferID = countValute.TransactionsID; // Добавляем корректный Transfer

                        }
                        else
                        {
                            transactionsBetween.ValuteTransactions = cardSender.TypeMoney;

                            transactionsBetween.Expenses = value;// Расход в валюту
                            transactionsBetween.Arrival = value;// Приход в валюте

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

                        transferClient.Transactions = transactions;

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

    }
}
