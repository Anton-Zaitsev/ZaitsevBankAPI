using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ZaitsevBankAPI.Models
{
    public class Exchange
    {
        [Key]
        public string IDValute { get; set; }
        [Required]
        public string CharCode { get; set; }
        [Required]
        public string NameValute { get; set; }
        [Required]
        public bool ChangesBuy { get; set; }
        [Required]
        public double ValuteBuy { get; set; }
        [Required]
        public bool ChangesSale { get; set; }
        [Required]
        public double ValuteSale { get; set; }
        [Required]
        public bool ElectronValute { get; set; }
    }


}
