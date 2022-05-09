using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models.TransactionsModels
{
    public class TransferClient
    {
        [Key]
        [Required]
        public Guid TransactionsID { get; set; } = Guid.NewGuid();
        [Required]
        public int NumberDocument { get; set; }
        [Required]
        public Guid TransactionSender { get; set; } // Отправитель
        [Required]
        public Guid TransactionRecipient { get; set; } // Получатель

        public string NameSender { get; set; } // Имя Отправителя
        public string NameRecipient { get; set; } // Получателя

        [Required]
        public Guid UserSender { get; set; }
        [Required]
        public Guid UserRecipient { get; set; }

        public virtual ICollection<Transactions> Transactions { get; set; }  
    }
}
