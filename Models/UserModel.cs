using System.ComponentModel.DataAnnotations;
namespace ZaitsevBankAPI.Models
{
    public class UserModel
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required, DataType(DataType.EmailAddress)]
        public string Login { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public string Birthday { get; set; }

        public string Gender { get; set; }

        [Required, DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

    }
 
}
