using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Services;


namespace ZaitsevBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }
        //https://localhost:7008/api/Users/Get
        [HttpGet]
        public async Task<IActionResult> Sign()
        {
            try
            {
                StringValues headerValueLogin = Request.Headers["login"];
                if (headerValueLogin.Any() == false) return BadRequest(); //401

                StringValues headerValuePass = Request.Headers["password"];
                if (headerValuePass.Any() == false) return BadRequest(); //401

                string Login = headerValueLogin.ToString();
                string Password = headerValuePass.ToString();
                if (RegexUtilities.IsValidEmail(Login) == false) return StatusCode(412, "Не верный email");

                UserService userService = new();
                var data = await userService.SignIn(Login, Password);
                if (data == null) return NotFound();

                return Ok(data);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Внутренняя ошибка сервера " + e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserData(string userID)
        {
            try
            {
                UserService userService = new();
                var data = await userService.GetUserData(userID);
                if (data == null) return NotFound();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(string FirstName, string LastName, string MiddleName, string Birthday, string Gender)
        {
            try
            {
                StringValues headerValueLogin = Request.Headers["login"];
                if (headerValueLogin.Any() == false) return BadRequest(); //401

                StringValues headerValuePass = Request.Headers["password"];
                if (headerValuePass.Any() == false) return BadRequest(); //401

                StringValues headerValuePhone = Request.Headers["phone"];
                if (headerValuePhone.Any() == false) return BadRequest(); //401

                string Login = headerValueLogin.ToString();
                string Password = headerValuePass.ToString();
                string Phone = headerValuePhone.ToString();

                if (RegexUtilities.IsValidEmail(Login) == false) return StatusCode(412, "Не верный email");
                if (RegexUtilities.isValidPhone(Phone) == false) return StatusCode(412, "Не верный номер телефона");

                UserService userService = new();
                var data = await userService.CreateAccount(Login, Password, Phone, FirstName, LastName, MiddleName, Birthday, Gender);

                return data != null ? Ok(data) : StatusCode(412, "Такой пользователь уже существует");

            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
