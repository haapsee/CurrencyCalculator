using System;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Services {
    public class RequestsService
    {
        private readonly HttpClient _httpClient;

        public RequestsService()
        {
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
                var apiKey = Environment.GetEnvironmentVariable("CURRENCYFREAKS_APIKEY");
                String url = "https://api.currencyfreaks.com/v2.0/rates/latest?apikey=" + apiKey;
                
                if (from.Equals("USD")) {
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    String res = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<RatesResponse>(res);
                    return Convert.ToDecimal(json.rates[to]);
                }
                else if (to.Equals("USD")) {
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    String res = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<RatesResponse>(res);
                    Console.WriteLine(json.rates[from]);
                    return 1.0m / Convert.ToDecimal(json.rates[from]);
                }
                else {
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    String res = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<RatesResponse>(res);
                    decimal fromRate = Convert.ToDecimal(json.rates[from]);
                    decimal toRate = Convert.ToDecimal(json.rates[to]);
                    return (1.0m / fromRate) * toRate;
                }
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
