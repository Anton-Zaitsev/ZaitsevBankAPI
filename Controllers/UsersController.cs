using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<string> Get()
        {
            await Task.Delay(100);
            return "connect";
        }

        [HttpGet]
        public async Task<string> GetHelp(int id)
        {
            await Task.Delay(100);
            return "connect + "  + id.ToString() ;
        }

    }
}
