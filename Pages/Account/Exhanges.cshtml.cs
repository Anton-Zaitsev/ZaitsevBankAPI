using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZaitsevBankAPI.Models;
using ZaitsevBankAPI.Services;

namespace ZaitsevBankAPI.Pages.Account
{
    public class ExhangesModel : PageModel
    {
        private readonly ILogger<ExhangesModel> _logger;
        public string dataNow { get; set; } = "";

        private readonly ExchangesService service = new();
        public List<Exchange> exchanges { get; set; } = new();
        public List<Exchange> exchangesElectron { get; set; } = new();
        public ExhangesModel(ILogger<ExhangesModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var list = await service.GetExchangeList(false);
            var listElectron = await service.GetExchangeList(true);
            if (list == null || listElectron == null) return NotFound();
            exchanges = list;
            exchangesElectron = listElectron;
            dataNow = DateTime.Now.ToString("D");
            return Page();
        }
    }
}
