using Microsoft.EntityFrameworkCore;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Models;

namespace ZaitsevBankAPI.Services
{
    public class UserService
    {
        private readonly ApplicationContext _context;
        public UserService()
        {
            _context = new();
        }

        public async Task<SignUserModel?> SignIn(string login, string password)
        {
            string password_sha256 = EncryptionBank.sha256(password);
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Login == login && user.Password == password_sha256);
            if (user == null) return null;
            return new SignUserModel { NameUser = user.FirstName, UserID = user.UserID };
        }
        public async Task<bool> CheckUserFind(string userID)
        {
            var id = Guid.Parse(userID);
            return  await _context.Users.FindAsync(id) != null;
        }
        public async Task<UserModel?> GetUserData(string userID)
        {
            var id = Guid.Parse(userID);
            return await _context.Users.FindAsync(id);
        }
        public async Task<SignUserModel?> CreateAccount(string Login, string Password, string Phone, string FirstName, string LastName, string? MiddleName, string Birthday, string Gender, string ip = "")
        {
            var data = await _context.Users.Where(user => user.Login == Login || user.Phone == Phone).ToListAsync();
            if (data.Count == 0)
            {
                UserModel user = new()
                {
                    Login = Login,
                    Password = EncryptionBank.sha256(Password),
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    Birthday = Birthday,
                    Gender = Gender,
                    MiddleName = MiddleName
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return new SignUserModel { NameUser = user.FirstName, UserID = user.UserID };
            }
            else
            {
                return null;
            }
        }
    }
   
}

    
