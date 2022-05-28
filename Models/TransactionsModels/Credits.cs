using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models.TransactionsModels
{
    public class Credits
    {
        [Key]
        [Required]
        public Guid TransactionsID { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public int NumberDocument { get; set; }
        [Required]
        public Guid TransactionCard { get; set; }
        [Required]
        public double CreditSumm { get; set; }
        [Required]
        public float Rate { get; set; } // Процентная ставка
        [Required]
        public int Period { get; set; } // Период в месяцах

        public virtual ICollection<Transactions> Transactions { get; set; }
    }

    public class PaymentsCredit
    {
        public int month { get; set; }
        public double pay { get; set; }

        public double percents { get; set; }

        // Остаток по кредиту
        public double lastSumm { get; set; }
    }
    public class CreditCheck
    {
        public List<PaymentsCredit> paymentsCredits { get; set; }

        public double summCredit { get; set; }

        public int monthCredit { get; set; }

        public double persent { get; set; }

        // Переплата
        public float overPayment { get; set; } = 0;
        public float monthlyPayment { get; set; }
    }
}
