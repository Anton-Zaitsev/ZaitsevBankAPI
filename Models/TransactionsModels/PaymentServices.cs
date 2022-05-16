using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models.TransactionsModels
{
    public class PaymentServices
    {
        [Key]
        [Required]
        public Guid TransactionsID { get; set; } = Guid.NewGuid();
        [Required]
        public int NumberDocument { get; set; }
        [Required]
        public Guid DebitCard { get; set; }
        [Required]
        public Guid UserID { get; set; } // Пользователь который отправляет / Получает
        [Required]
        public string NameServices { get; set; }

        public string CorrespondentScore { get; set; } // Специальный счет для организаций которые оплачивают вам
        public bool? Payment { get; set; } // Покупка или прибыль
        public virtual Transactions Transactions { get; set; }
    }
}
