using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZaitsevBankAPI.Services;

namespace ZaitsevBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuteController : Controller
    {
        private readonly ILogger<ValuteController> _logger;
        public ValuteController(ILogger<ValuteController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateValuteDataBase()
        {
            ExchangesService exchangesService = new();
            if (await exchangesService.ExchangesUpdate())
            {
                return Ok();
            }
            else
            {
                return BadRequest(); //401
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPopListValute(bool ElectronValute)
        {
            ExchangesService exchangesService = new();
            var list = await exchangesService.GetPopularList(ElectronValute);
            return list != null ? Ok(list) : NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> ValuteATOValuteB(string ValuteA, string ValuteB, bool BuySale, double count = 1)
        {
            ExchangesService exchangesService = new();
            var change = await exchangesService.ValuteATOValuteB(ValuteA, ValuteB, BuySale, count);
            return change != null ? Ok(change) : NotFound();
        }
    }
}
