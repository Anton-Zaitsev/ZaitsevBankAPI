using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models
{
    public class CardModel
    {
        [Key]
        public Guid CardID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public string CardOperator { get; set; }

        [Required]
        public bool ClosedCard { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        public DateTime DataClosedCard { get; set; }
        [Required]
        public double MoneyCard { get; set; }
        [Required]
        public string NameCard { get; set; }
        [Required]
        public string NumberCard { get; set; }
        [Required]
        public string TypeMoney { get; set; }
        [Required]
        public Guid TransactionCard { get; set; } = Guid.NewGuid();
    }

}
