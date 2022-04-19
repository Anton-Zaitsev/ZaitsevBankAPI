using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using ZaitsevBankAPI.Models;

namespace ZaitsevBankAPI.Services
{
    public class ExchangesService
    {
        private readonly ApplicationContext _context;
        public ExchangesService()
        {
            _context = new();
        }
        
        public async Task<List<Exchange>?> GetExchangeList(bool ElectronValute)
        {
            var list = await _context.Exchanges.Where(x => x.ElectronValute == ElectronValute).ToListAsync();
            return list.Count == 0 ? null : list;
        }

        public async Task<ExhangeValuteAtoValuteB?> ValuteATOValuteB(string ValuteA, string ValuteB, bool BuySale,double? CountValute)
        {
            // Конверт из A -> Б
            // Count принадлжит валюты А
            // BuySale - true -- ПОКУПКА

            FunctionBank.CreditCard creditCard = new();
            var valuteTypeA =  creditCard.isValidValuteType(ValuteA);
            var valuteTypeB =  creditCard.isValidValuteType(ValuteB);
            if (valuteTypeA == null || valuteTypeB == null) return null;
            // Электронные валюты всегда представлены в долларах
            // Обычные валюты вседа представлены в рублях

            if (creditCard.isElectronValute(ValuteA) && creditCard.isElectronValute(ValuteB) == false) // Карта типа А Электронная валюта и B обычная
            {
                double count = CountValute ?? 1; // Default
                ExhangeValuteAtoValuteB exhange = new();
                exhange.actualDateTime = DateTime.Now;
                exhange.Count = count;

                var valuteA = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteA); // ВалютаА всегда в долларах
                if (valuteA == null) return null;

                if (ValuteB == "USD")
                {
                    exhange.ValuteConvert = BuySale ? Math.Round(valuteA.ValuteBuy * count, 4) : Math.Round(valuteA.ValuteSale * count, 4);
                    return exhange;

                }
                else if (ValuteB == "RUB")
                {
                    var usd = await _context.Exchanges.FindAsync("R01235"); //Находим доллар в рублях
                    if (usd == null) return null;
                    exhange.ValuteConvert = BuySale ? Math.Round((valuteA.ValuteBuy * count) * usd.ValuteBuy, 4) : Math.Round((valuteA.ValuteSale * count) * usd.ValuteBuy, 4);
                    return exhange;
                }
                else
                {
                    var usd = await _context.Exchanges.FindAsync("R01235"); //Находим доллар в рублях
                    if (usd == null) return null;
                    var valuteB = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteB); // Находим валюту, которую мы ищем
                    if (valuteB == null) return null;


                    double valuteBConvert = (BuySale ? valuteB.ValuteBuy : valuteB.ValuteSale * count) / usd.ValuteBuy;
                    double bitValute = (valuteA.ValuteBuy * count);
                    exhange.ValuteConvert = BuySale ? Math.Round(bitValute / valuteBConvert, 4) : Math.Round(bitValute / valuteBConvert, 4);
                    return exhange;
                }   

            }
            else if (creditCard.isElectronValute(ValuteA) == false && creditCard.isElectronValute(ValuteB))// Карта типа A обычная и B электронная
            {
                ExhangeValuteAtoValuteB exhange = new();
                exhange.actualDateTime = DateTime.Now;

                var valuteB = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteB); // ВалютаB всегда в долларах
                if (valuteB == null) return null;

                if (ValuteA == "USD")
                {
                    double count = CountValute ?? 1000; // Default to USD
                    exhange.Count = count;
                    exhange.ValuteConvert = BuySale ? (1 * count) / valuteB.ValuteBuy : (1 * count) / valuteB.ValuteSale;
                    return exhange;

                }
                else if (ValuteA == "RUB")
                {
                    var usd = await _context.Exchanges.FindAsync("R01235"); //Находим доллар в рублях
                    if (usd == null) return null;

                    double count = CountValute ?? 100000; // Default to RUB
                    exhange.Count = count;

                    double rub =  (1 * count) / usd.ValuteBuy;
                    exhange.ValuteConvert = BuySale ? rub / valuteB.ValuteBuy : rub / valuteB.ValuteSale;
                    return exhange;
                }
                else
                {
                    var usd = await _context.Exchanges.FindAsync("R01235"); //Находим доллар в рублях
                    if (usd == null) return null;
                    var valuteA = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteA); // Находим валюту, которую мы ищем
                    if (valuteA == null) return null;

                    double count = CountValute ?? 1000; // Default
                    exhange.Count = count;

                    var valuteAConvert = (valuteA.ValuteBuy * count) / usd.ValuteBuy;
                    // Переводим электронную валюту в rub посредством продажи доллара и продажи битка
                    // Находим 
                    exhange.ValuteConvert = BuySale ? valuteAConvert / valuteB.ValuteBuy : valuteAConvert / valuteB.ValuteSale;
                    return exhange;

                }

            }
            else if (creditCard.isElectronValute(ValuteA) && creditCard.isElectronValute(ValuteB)) // Обе карты электронные валюты
            {
                ExhangeValuteAtoValuteB exhange = new();
                exhange.actualDateTime = DateTime.Now;

                var valuteA = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteA);
                if (valuteA == null) return null;
                var valuteB = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteB);
                if (valuteB == null) return null;

                // Например 1 биткойн в Thether
                double count = CountValute ?? 1; // Default
                exhange.Count = count;
                exhange.ValuteConvert = BuySale ? Math.Round((valuteA.ValuteBuy * count) / valuteB.ValuteBuy, 4) : Math.Round((valuteA.ValuteBuy * count) / valuteB.ValuteSale, 4);
                return exhange;

            }
            else if (creditCard.isElectronValute(ValuteA) == false && creditCard.isElectronValute(ValuteB) == false) // Обе карты обычные валюты
            {
                ExhangeValuteAtoValuteB exhange = new();
                exhange.actualDateTime = DateTime.Now;
                double count = CountValute ?? 1; // Default
                exhange.Count = count;

                if (ValuteA == "RUB" && ValuteB != "RUB") // --> Рубли в Доллары допустим
                {
                    var valuteB = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteB);
                    if (valuteB == null) return null;
                    exhange.ValuteConvert = BuySale ? Math.Round((1 * count) / valuteB.ValuteBuy, 4) : Math.Round((1 * count) / valuteB.ValuteSale, 4);
                    return exhange;
                }
                else if (ValuteA != "RUB" && ValuteB == "RUB") // --> Доллары в рубли допустим
                {
                    var valuteA = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteA);
                    if (valuteA == null) return null;
                    exhange.ValuteConvert = BuySale ? Math.Round(valuteA.ValuteBuy * count, 4) : Math.Round(valuteA.ValuteSale * count, 4);
                    return exhange;
                }
                else if (ValuteA == "RUB" && ValuteB == "RUB")
                {
                    exhange.ValuteConvert = Math.Round(1 * count, 4);
                    return exhange;
                }
                else // --> Доллары в ЕВРО допустим
                {
                    var valuteA = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteA);
                    if (valuteA == null) return null;
                    var valuteB = await _context.Exchanges.FirstOrDefaultAsync(valute => valute.CharCode == ValuteB);
                    if (valuteB == null) return null;
                    exhange.ValuteConvert = BuySale ? Math.Round((valuteA.ValuteBuy * count) / valuteB.ValuteBuy, 4) : Math.Round((valuteA.ValuteBuy * count) / valuteB.ValuteSale, 4);
                    return exhange;  // Например 1 Евро в $ 

                }
            }
            else
            {
                return null; // Не найдены типы карты
            }
        }
        public async Task<List<Exchange>?> GetPopularList(bool ElectronValute)
        {
            List<Exchange> list = new();
            if (ElectronValute)
            {
                var BitCoin = await _context.Exchanges.FindAsync("bitcoin");
                if (BitCoin != null)
                {
                    list.Add(BitCoin);
                }
                var Ethereum = await _context.Exchanges.FindAsync("ethereum");
                if (Ethereum != null)
                {
                    list.Add(Ethereum);
                }
                var Tether = await _context.Exchanges.FindAsync("tether");
                if (Tether != null)
                {
                    list.Add(Tether);
                }
            }
            else
            {
                var USD = await _context.Exchanges.FindAsync("R01235");
                if (USD != null)
                {
                    list.Add(USD);
                }
                var EUR = await _context.Exchanges.FindAsync("R01239");
                if (EUR != null)
                {
                    list.Add(EUR);
                }
                var UAH = await _context.Exchanges.FindAsync("R01720");
                if (UAH != null)
                {
                    list.Add(UAH);
                }
            }
            return list.Count > 0 ? list : null;
        }
        public async Task<bool> ExchangesUpdate()
        {

            var listExchange = await GetExchanges();
            if (listExchange == null) return false;
            var listExchangeElectron = await GetExchangesElectronValute();
            if (listExchangeElectron == null) return false;

            var listAllList = listExchange.Concat(listExchangeElectron).OrderByDescending(x => x.CharCode).ToList(); // Сортировка по CharCode


            var ExchangesDB = await _context.Exchanges.ToListAsync();

            if (ExchangesDB.Count == 0)
            {
                await _context.Exchanges.AddRangeAsync(listAllList);
                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                foreach (var Exchange in listAllList)
                {
                    var index = ExchangesDB.FindIndex(x => x.IDValute == Exchange.IDValute);
                    if (index != -1)
                    {
                        ExchangesDB[index].ChangesBuy = Exchange.ChangesBuy;
                        ExchangesDB[index].ChangesSale = Exchange.ValuteSale > ExchangesDB[index].ValuteSale ;
                        ExchangesDB[index].ValuteBuy = Math.Round(Exchange.ValuteBuy, ExchangesDB[index].ElectronValute ? 6 : 4);
                        ExchangesDB[index].ValuteSale = Math.Round(Exchange.ValuteSale, ExchangesDB[index].ElectronValute ? 6 : 4);
                    }
                }
                _context.Exchanges.UpdateRange(ExchangesDB);
                await _context.SaveChangesAsync();
                GC.Collect();
                return true;
            }

        }

        private async Task<List<Exchange>?> GetExchanges()
        {
            try
            {
                using (HttpClient client = new())
                {
                    string request = "https://www.cbr-xml-daily.ru/daily_json.js";
                    List<Exchange> list = new();
                    Random rnd = new();

                    client.Timeout = new TimeSpan(0, 5, 0); // 5 минут   

                    HttpResponseMessage httpResponse = await client.GetAsync(request);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string json = await httpResponse.Content.ReadAsStringAsync();
                        var objectJson = JObject.Parse(json);
                        if (objectJson == null) return null;
                        Dictionary<string, object>? dict = objectJson.SelectToken("Valute").ToObject<Dictionary<string, object>>();
                        if (dict == null) return null;
                        foreach (KeyValuePair<string, object> valutes in dict)
                        {
                            if (valutes.Value is JToken) {
                                var valute = (JToken)valutes.Value;
                                var ID = (string?)valute["ID"];
                                var CharCode = (string?)valute["CharCode"];
                                var Nominal = (int?)valute["Nominal"];
                                var Name = (string?)valute["Name"];
                                var Value = (double?)valute["Value"];
                                var Previous = (double?)valute["Previous"];

                                if (ID == null || CharCode == null || Nominal == null || Name == null || Value == null || Previous == null)
                                    continue;
                                else
                                {
                                    double valuteBuy = Math.Round(Value.Value / Nominal.Value, 4);
                                    double valuteSale = Math.Round(GetNextSaleValute(rnd, valuteBuy), 4); // Сокращения знака до 4 и Рандомное число от 1.5 до 10
                                    Exchange exchange = new()
                                    {
                                        IDValute = ID,
                                        NameValute = Name,
                                        CharCode = CharCode,
                                        ChangesBuy = Value > Previous,
                                        ValuteBuy = valuteBuy,
                                        ChangesSale = GetRandomBool(rnd),
                                        ValuteSale = valuteSale,
                                        ElectronValute = false
                                    };
                                    list.Add(exchange);
                                }
                            }
                        }

                        if (list.Count == 0) return null;

                        return list;

                    }
                    else
                    {
                        return null;
                    }
                }
                
            }
            catch (Exception)
            {
                return null;
            }
        }
        private async Task<List<Exchange>?> GetExchangesElectronValute()
        {
            try
            {
                using (HttpClient client = new())
                {
                    string requestString = "https://api.coincap.io/v2/assets";
                    List<Exchange> list = new();
                    Random rnd = new();

                    client.Timeout = new TimeSpan(0, 5, 0); // 5 минут   

                    using (var request = new HttpRequestMessage(HttpMethod.Get, requestString))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "51e9fe33-fffe-4a31-83b0-19a230a712b7"); //Справа токен
                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            //ValuteCb valuteCb = JsonConvert.DeserializeObject<ValuteCb>(json);
                            var objectJson = JObject.Parse(json);
                            if (objectJson == null) return null;
                            var dict = (JArray?)objectJson["data"];
                            if (dict == null) return null;
                            foreach (var valute in dict)
                            {
                                var ID = (string?)valute["id"];
                                var CharCode = (string?)valute["symbol"];
                                var Name = (string?)valute["name"];
                                var Value = (double?)valute["priceUsd"];
                                var Changes = (double?)valute["changePercent24Hr"];

                                if (ID == null || CharCode == null || Name == null || Value == null || Changes == null)
                                    continue;
                                else
                                {
                                    double valuteBuy = Math.Round(Value.Value, 6);
                                    double valuteSale = Math.Round(GetNextSaleValute(rnd, valuteBuy), 6); // Сокращения знака до 4 и Рандомное число от 1.5 до 10
                                    Exchange exchange = new()
                                    {
                                        IDValute = ID,
                                        NameValute = Name,
                                        CharCode = CharCode,
                                        ChangesBuy = Changes > 0,
                                        ValuteBuy = valuteBuy,
                                        ChangesSale = GetRandomBool(rnd),
                                        ValuteSale = valuteSale,
                                        ElectronValute = true
                                    };
                                    list.Add(exchange);
                                }
                                
                            }

                            if (list.Count == 0) return null;
                            return list;

                        }
                        else
                        {
                            return null;
                        }
                    }
                    
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static double GetNextSaleValute(Random random, double valueBuy)
        {
            int procent = random.Next(8, 15); // ПРоцент на повышение от покупки для продажи от 10 до 15
            return (valueBuy * (procent + 100)) / 100;
        }
        private static bool GetRandomBool(Random random)
        {
            return random.Next(2) == 1;
        }
    }
}
