using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models.TransactionsModels
{
    public class CurrencyTransfer
    {
        [Key]
        [Required]
        public Guid TransactionsID { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public int NumberDocument { get; set; }
        [Required]
        public Guid TransactionCardFrom { get; set; }
        [Required]
        public Guid TransactionCardTo { get; set; }
        [Required]
        public string ValuteA { get; set; }
        [Required]
        public string ValuteB { get; set; }
        [Required]
        public bool BuySaleValute { get; set; } = true; // Дефолт покупка в банке валюты

        [Required]
        public double ActualCurseRub { get; set; }

        public virtual Transactions Transactions { get; set; }
    }
}
