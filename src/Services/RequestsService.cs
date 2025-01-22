using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Services {
    public class RequestsService
    {
        private readonly HttpClient _httpClient;
        private ICacheService cacheService;

        public RequestsService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
            var apiKey = Environment.GetEnvironmentVariable("CURRENCYFREAKS_APIKEY");
            if (apiKey == null)
            {
                throw new InvalidOperationException("API key is not set in environment variables.");
            }

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<decimal> GetExchangeRate(string from, string to)
        {
            try
            {
                decimal currencyRate;

                currencyRate = this.cacheService.GetConversionRate(from, to);
                if (currencyRate != 0)
                {
                    Console.WriteLine("From cache");
                    return currencyRate;
                }

                if (!to.Equals("USD") && !from.Equals("USD")) {
                    currencyRate = await GetExchangeRate("USD", to) * await GetExchangeRate(from, "USD");
                } else {
                    var apiKey = Environment.GetEnvironmentVariable("CURRENCYFREAKS_APIKEY");
                    String url = "https://api.currencyfreaks.com/v2.0/rates/latest?apikey=" + apiKey;

                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    String res = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<RatesResponse>(res);

                    if (from.Equals("USD")) {
                        currencyRate = Convert.ToDecimal(json.rates[to]);
                    } else {
                        currencyRate = 1m / Convert.ToDecimal(json.rates[from]);
                    }
                }
                // Save the conversion rate to cache for future use
                this.cacheService.SaveConversionRate(from, to, currencyRate);
                Console.WriteLine($"From cache: {currencyRate}");
                return currencyRate;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

    internal class RatesResponse
    {
        public Dictionary<string, string> rates { get; set; }
    }
}
