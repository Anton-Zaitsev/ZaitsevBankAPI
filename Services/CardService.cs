using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models;

namespace ZaitsevBankAPI.Services
{
    public class CardService
    {
        private readonly int timeActivateCard = 4; // 4 года дейсвия карты

        private readonly ApplicationContext _context;
        public CardService()
        {
            _context = new();
        }
        public async Task<bool> CreateCard(string userID, string cardOperator,string nameCard, string typeMoney)
        {
            UserService userService = new();
            if (await userService.CheckUserFind(userID) == false) return false; 

            CreditCard creditCard = new();
            var numberCard = creditCard.generateCardNumber(cardOperator);
            if (numberCard == null) return false;
            typeMoney = typeMoney.ToUpper();
            if (creditCard.isValidValuteType(typeMoney) == null) return false;
            string CVV_card = creditCard.generateCVV();
            Guid id = Guid.Parse(userID);
            DateTime timeClosedCard = DateTime.Now.AddYears(timeActivateCard);

            Guid transationCardID = Guid.NewGuid();

            CardModel card = new()
            {
                UserID = id,
                CardOperator = cardOperator,
                ClosedCard = false,
                CVV = CVV_card,
                DataClosedCard = timeClosedCard,
                MoneyCard = 0.0,
                NameCard = nameCard,
                NumberCard = numberCard,
                TypeMoney = typeMoney,
                TransactionCard = transationCardID
            };
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
            string infoCard = nameCard + " •• " + numberCard.Substring(numberCard.Length - 4);
            await new TransactionsServices.TransactionsCardService().CreateCard(id, transationCardID, infoCard);
            return true;
        }
        
        public async Task<List<CardModel>?> GetAllCards(string userID)
        {
            Guid id = Guid.Parse(userID);
            var Cards = await _context.Cards.Where(data => data.UserID == id).ToListAsync();
            if (Cards == null) return null;
            if (Cards.Count == 0) return null;    
            return Cards;
        }

        public async Task<List<CardModel>?> GetAllCardFilter(string userID, string CardFilter)
        {
            Guid id = Guid.Parse(userID);
            Guid cardFilter = Guid.Parse(CardFilter);
            var Cards = await _context.Cards.Where(data => data.UserID == id && data.TransactionCard != cardFilter).ToListAsync();
            if (Cards == null) return null;
            if (Cards.Count == 0) return null;
            return Cards;
        }
        public async Task<List<CardModel>?> GetCardsBuySale(string userID, string TypeValute, bool BuySale)
        {
            DateTime time = DateTime.Now;
            Guid id = Guid.Parse(userID);
            var Cards = await _context.Cards.Where( card => card.UserID == id && 
            card.DataClosedCard > time && (BuySale ? card.TypeMoney != TypeValute : card.TypeMoney == TypeValute)).ToListAsync();
            // Если BuySale по дефолту это true, то ищем карты, которые не равны валюты из запроса, если false, то карты которые нужны для продажи
            if (Cards == null) return null;
            if (Cards.Count == 0) return null;
            return Cards;
        }

        public async Task<CardSearch?> GetCardFromPhone(string phone, string idUser)
        {
            Guid id = Guid.Parse(idUser);
            var user = await _context.Users.FirstOrDefaultAsync(data => data.Phone == phone);
            if (user == null) return null;

            var card = await _context.Cards.FirstOrDefaultAsync(y => y.UserID == user.UserID);
            if (card == null) return null;
            var cardUser = await _context.Cards.Where(data => data.UserID == id && data.TransactionCard != card.TransactionCard).ToListAsync();
            if (cardUser == null) return null;
            if (cardUser.Count == 0) return null;

            var cardFilter = cardUser.Where(data => data.TypeMoney == card.TypeMoney).FirstOrDefault() ?? cardUser.FirstOrDefault(); // Если карта не найдена по нашей валюте, то предложим перевод на другие валюты
            if (cardFilter == null) return null;

            string middlename = user.MiddleName == null ? "" : $"{user.MiddleName} ";
            string lastName = user.MiddleName == null ? user.LastName : $"{user.LastName.First()}.";
            string nameUser = user.FirstName + " " + middlename + lastName;

            CardSearch cardDataSearch = new()
            {
                NameUser = nameUser,
                TransactionCard = card.TransactionCard,
                PhoneNumber = user.Phone,
                IdeticalValute = cardFilter.TypeMoney == card.TypeMoney,
                ValuteSender = cardFilter.TypeMoney,
                ValuteReceiver = card.TypeMoney,
                CardFirst = cardFilter

            };
            return cardDataSearch;
        }
        public async Task<CardSearch?> GetCardFromNumber (string numberCard,string idUser)
        {
            Guid id = Guid.Parse(idUser);
            string cardNumber = numberCard.Trim();
            var card = await _context.Cards.FirstOrDefaultAsync(y => y.NumberCard == cardNumber);
            if (card == null) return null;
            var cardUser = await _context.Cards.Where(data => data.UserID == id && data.TransactionCard != card.TransactionCard).ToListAsync(); //&& card.TransactionCard != card.TransactionCard Дополнительная проверка на карту
            if (cardUser == null) return null;
            if (cardUser.Count == 0) return null;

            var user = await _context.Users.FindAsync(card.UserID);
            if (user == null) return null;

            var cardFilter = cardUser.Where(data => data.TypeMoney == card.TypeMoney).FirstOrDefault() ?? cardUser.FirstOrDefault(); // Если карта не найдена по нашей валюте, то предложим перевод на другие валюты
            if (cardFilter == null) return null;

            string middlename = user.MiddleName == null ? "" : $"{user.MiddleName} ";
            string lastName = user.MiddleName == null ? user.LastName : $"{user.LastName.First()}.";
            string nameUser = user.FirstName + " " + middlename + lastName;

            CardSearch cardDataSearch = new()
            {
                NameUser = nameUser,
                TransactionCard = card.TransactionCard,
                PhoneNumber = user.Phone,
                IdeticalValute = cardFilter.TypeMoney == card.TypeMoney,
                ValuteSender = cardFilter.TypeMoney,
                ValuteReceiver = card.TypeMoney,
                CardFirst = cardFilter

            };
            return cardDataSearch;
        }

        public async Task<CardModel?> GetCardClient(Guid transactionCard)
        {
            return await _context.Cards.FirstOrDefaultAsync(data => data.TransactionCard == transactionCard);
        }
    }
}
