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
        public DateTime Period { get; set; } // Период

        public virtual ICollection<Transactions> Transactions { get; set; }
    }

    public class CreditCheck
    {
        public double CreditSumm { get; set; }

        public int Period { get; set; } // Период в годах
        public float Rate { get; set; } // Процентная ставка
    }
}
