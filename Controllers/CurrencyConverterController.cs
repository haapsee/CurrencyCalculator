using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Services;


namespace CurrencyConverter.controllers {
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController {
        private RequestsService requestsService;
        public CurrencyConverterController(ICacheService cacheController) {
            this.requestsService = new RequestsService(cacheController);
        }

        [HttpGet()]
        public async Task<ActionResult<decimal>> Get(string from, string to, decimal amount) {
            var response = await requestsService.GetExchangeRate(from, to);
            decimal exchangeRate = response;
            return amount * exchangeRate;
        }
    }
}