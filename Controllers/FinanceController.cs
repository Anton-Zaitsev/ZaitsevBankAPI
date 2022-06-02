using Microsoft.AspNetCore.Mvc;
using ZaitsevBankAPI.Services.TransactionsServices;

namespace ZaitsevBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly ILogger<FinanceController> _logger;
        public FinanceController(ILogger<FinanceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetFinanceMonth(string userID)
        {
            ExpensesGetList expensesGetList = new();
            var finance = await expensesGetList.ExpensesAsync(userID);
            return finance != null ? Ok(finance) : StatusCode(412, "Нет трат");
        }
    }
}
