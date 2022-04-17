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
            CreditCard creditCard = new();
            var numberCard = creditCard.generateCardNumber(cardOperator);
            if (numberCard == null) return false;
            typeMoney = typeMoney.ToUpper();
            if (creditCard.isValidValuteType(typeMoney) == null) return false;
            string CVV_card = creditCard.generateCVV();
            var id = Guid.Parse(userID);
            DateTime timeClosedCard = DateTime.Now.AddYears(timeActivateCard);

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
                TypeMoney = typeMoney
            };
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<CardModel>?> GetAllCards(string userID)
        {
            var id = Guid.Parse(userID);
            var Cards = await _context.Cards.Where(data => data.UserID == id).ToListAsync();
            if (Cards == null) return null;
            if (Cards.Count == 0) return null;

            DateTime time = DateTime.Now;
            bool creditClosed = false;
            for(int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].DataClosedCard <= time) // Если дейсвительность карты меньше или равно текущему времени, то закрываем карту!
                {
                    Cards[i].ClosedCard = true;
                    creditClosed = true;
                }
            }
            if (creditClosed) await _context.SaveChangesAsync();
            return Cards;
        }
        public async Task<List<CardModel>?> GetCardsBuySale(string userID, string TypeValute, bool BuySale)
        {
            DateTime time = DateTime.Now;
            var id = Guid.Parse(userID);
            var Cards = await _context.Cards.Where( card => card.UserID == id && 
            card.DataClosedCard > time && (BuySale ? card.TypeMoney != TypeValute : card.TypeMoney == TypeValute)).ToListAsync();
            // Если BuySale по дефолту это true, то ищем карты, которые не равны валюты из запроса, если false, то карты которые нужны для продажи
            if (Cards == null) return null;
            if (Cards.Count == 0) return null;
            return Cards;
        }
        public async Task<CardSearch?> GetCardFromPhone(string phone, string typeValute)
        {
            if (new CreditCard().isValidValuteType(typeValute.ToUpper()) != null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == phone);
            if (user == null) return null;
            var card = await _context.Cards.FirstOrDefaultAsync(y => y.UserID == user.UserID && y.TypeMoney == typeValute);
            if (card == null) return null;
            string middlename = user.MiddleName == null ? "" : $"{user.MiddleName} ";
            string lastName = user.MiddleName == null ? user.LastName : $"{user.LastName.First()}.";
            string nameUser = user.FirstName + " " + middlename + lastName;

            CardSearch cardDataSearch = new()
            {
                NameUser = nameUser,
                TransactionCard = card.TransactionCard,
                PhoneNumber = user.Phone
            };
            return cardDataSearch;
        }
    }
}
