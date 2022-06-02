namespace ZaitsevBankAPI.Models
{
    public class Expenses
    {
        public string valuteType { get; set; }

        public double summ { get; set; } = 0;

        public double transfers { get; set; } = 0; // Категория переводов количество для линии заполнения

        public double buyValute { get; set; } = 0;

        public double saleValute { get; set; } = 0; 

        public double transferCredit { get; set;  } = 0; // Оплата кредита

        public double other { get; set; }  // Другие
    }
}
