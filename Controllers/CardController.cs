using Microsoft.AspNetCore.Mvc;
using ZaitsevBankAPI.FunctionBank;
using ZaitsevBankAPI.Services;

namespace ZaitsevBankAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ILogger<CardController> _logger;
        public CardController(ILogger<CardController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(string userID, string CardOperator, string NameCard, string TypeMoney)
        {
            try
            {
                CardService cardService = new();
                bool completed = await cardService.CreateCard(userID, CardOperator, NameCard, TypeMoney);
                return completed ? Ok(): StatusCode(412, "Не удалось создать вашу карту");

            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards(string userID)
        {
            try
            {
                CardService cardService = new();
                var cards = await cardService.GetAllCards(userID);
                if (cards == null) return NotFound();
                return Ok(cards);

            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCardFromPhone(string Phone, string TypeValute)
        {
            try
            {
                if (RegexUtilities.isValidPhone(Phone))
                {
                    CardService cardService = new();
                    var card = await cardService.GetCardFromPhone(Phone, TypeValute);
                    if (card == null) return NotFound();
                    return Ok(card);
                }
                else
                {
                    return StatusCode(412, "Не правильный номер телефона");
                }

            }
            catch (Exception)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
