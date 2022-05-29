namespace ZaitsevBankAPI.Models
{
    public class CreditPay
    {
        public string numberDocument { get; set; }

        public DateTime dateCreditOffers { get; set; } // Дата кредита оформления

        public DateTime dateCreditEnd { get; set; } // Дата окончания кредита

        public List<CreditPaysTransaction> creditPaysTransaction { get; set; }
    }

    public class CreditPaysTransaction
    {
        public bool overdue { get; set; } // Просроченный 
        public bool waiting { get; set; } // Ожидающий прямо сейчас    

        public DateTime datePay { get; set; } // Дата кредита

        public double summCredit { get; set; }

        public double balanceCredit { get; set; }
    }
}
