namespace ZaitsevBankAPI.FunctionBank
{
    public class CreditCard
    {
        private enum CardOperator
        {
            MASTERCARD = 5,
            VISA = 4,
            MIR = 2
        }
        private List<string> CardValute =  new List<string> {
            "EUR",
            "USD",
            "RUB",
            "BTC",
            "ETH"
        };
        private readonly int lenghtCardNumber = 16;
        public string? generateCardNumber(string CardOperator)
        {
            CardOperator = CardOperator.ToUpper();
            bool correctOperator = ((CardOperator[])Enum.GetValues(typeof(CardOperator))).Select(c => c.ToString()).ToList().Contains(CardOperator);
            if (correctOperator == false) return null;
            int cardNumberFirst = (int)(CardOperator)Enum.Parse(typeof(CardOperator), CardOperator);
            string cardNumber = cardNumberFirst.ToString();
            Random rnd = new ();
            for (int i = 0; i < lenghtCardNumber - 1; i++)
            {
                cardNumber += rnd.Next(10).ToString(); // Создает число от 0 до 9
            }
            return cardNumber;
        }
        public string generateCVV()
        {
            return new Random().Next(100, 1000).ToString(); // Генерация CVV от 100 до 999, чтобы обязательно было 3 числа
        }
        public bool isValidValute(string typeValute)
        {
            return CardValute.Contains(typeValute);
        }
        public bool isValid(string NumberCard)
        {
            NumberCard = NumberCard.Trim();
            if (NumberCard.Length == lenghtCardNumber)
            {
                int cardNumberFirst;
                if (int.TryParse(NumberCard.FirstOrDefault().ToString(), out cardNumberFirst))
                {
                    List<int> myValuesCard = ((CardOperator[])Enum.GetValues(typeof(CardOperator))).Select(c => (int)c).ToList();
                    bool isValid = myValuesCard.Contains(cardNumberFirst);
                    return isValid;
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
