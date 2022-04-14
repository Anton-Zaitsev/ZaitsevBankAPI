using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            CardService cardService = new();
            bool completed = await cardService.CreateCard(userID, CardOperator, NameCard, TypeMoney);
            return completed ? Ok(): StatusCode(412, "Не удалось создать вашу карту");

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards(string userID)
        {

            CardService cardService = new();
            var cards = await cardService.GetAllCards(userID);
            if (cards == null) return NotFound();
            string json = JsonConvert.SerializeObject(cards);
            string encryptCard = EncryptionBank.EncryptCard(json,userID);
            string dec = EncryptionBank.DecryptCard(encryptCard, userID);
            return Ok(encryptCard);

        }

        [HttpGet]
        public async Task<IActionResult> GetCardFromPhone(string Phone, string TypeValute)
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

        [HttpGet]
        public async Task<IActionResult> GetCardsBuySale(string userID, string TypeValute, bool BuySale)
        {
            CardService cardService = new();
            var card = await cardService.GetCardsBuySale(userID, TypeValute, BuySale);  
            if (card == null) return NotFound();
            string json = JsonConvert.SerializeObject(card);
            string encryptCard = EncryptionBank.EncryptCard(json,userID);
            return Ok(encryptCard);
        }
    }
}
