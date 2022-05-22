using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models.TransactionsModels
{
    public class Transactions
    {
        [Key]
        [Required]
        public Guid TransactionsID { get; set; }
        [Required]
        public int CodeOperation { get; set; } // Код операции
        [Required]
        public string NameOperation { get; set; } // Название операции

        public DateTime? ArrivalDate { get; set; } // Дата Прихода
        public DateTime? ExpensesDate { get; set; } // Дата Расхода
        public double? Arrival { get; set; } // Приход
        public double? Expenses { get; set; } // Расход
        public string? ValuteTransactions { get; set; } // Валюта транзакции, не обязательна, если валюты были одинаковы при транзакции
        //Навигация
        public virtual Credits? Credits { get; set; }
        public virtual TransferClient? TransferClient { get; set; }
        public virtual CurrencyTransfer? CurrencyTransfer { get; set; }
        public virtual PaymentServices? PaymentServices { get; set; }
    }
}
