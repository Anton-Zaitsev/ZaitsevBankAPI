using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Pages.Models;
using ZaitsevBankAPI.Services;

namespace ZaitsevBankAPI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService userService = new();

        [BindProperty]
        public UserModelsSite UserData { get; set; } = new();
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void Get()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (UserData.Login == "berka@ya.ru")
            {
                var user = await userService.SignIn(UserData.Login, UserData.Password);
                return user == null ? NotFound("Пользователь не найден или не правильный пароль.") : RedirectToPage("./Account/Exhanges");

            }
            else
            {
                return NotFound("Пользователь не найден или не правильный пароль.");
            }
           
        }
    }
}