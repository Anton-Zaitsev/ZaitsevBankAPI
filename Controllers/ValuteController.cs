using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZaitsevBankAPI.Controllers
{
    public class ValuteController : Controller
    {
        private readonly ILogger<ValuteController> _logger;
        public ValuteController(ILogger<ValuteController> logger)
        {
            _logger = logger;
        }
    }
}
