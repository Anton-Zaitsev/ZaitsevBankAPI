namespace ZaitsevBankAPI.Models
{
    public class CardSearch
    {
        public string NameUser { get; set; }
        public Guid TransactionCard { get; set; }
        public string PhoneNumber { get; set; }

        public bool IdeticalValute { get; set; }

        public string ValuteSender { get; set; } // Валюта Отправителя

        public string ValuteReceiver { get; set; } // Валюта Получателя

        public CardModel CardFirst { get; set; }
    }
}
