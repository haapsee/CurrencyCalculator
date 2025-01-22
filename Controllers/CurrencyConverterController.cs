using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CurrencyConverter.Services;
using System.Threading.Tasks;


namespace CurrencyConverter.controllers {
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController {
        private CacheService cacheService;
        private RequestsService requestsService;
        public CurrencyConverterController() {
            this.cacheService = new CacheService();
            this.requestsService = new RequestsService();
        }


        [HttpGet()]
        public async Task<ActionResult<decimal>> Get(string from, string to, decimal amount) {
            Console.WriteLine(from + ", " + to + ", "+ amount);
            // Check if available cache for the given currency pair
            decimal exchangeRate = cacheService.GetConversionRate(from, to);
            Console.WriteLine(exchangeRate);

            if (exchangeRate == 0) {
                // Using CurrencyFreaks api and RequestsService for fetching the exchange rate
                var response = await requestsService.GetExchangeRate(from, to);
                exchangeRate = response;
                cacheService.SaveConversionRate(from, to, exchangeRate);
            }
            Console.WriteLine(exchangeRate);

            return amount * exchangeRate;
        }
    }
}