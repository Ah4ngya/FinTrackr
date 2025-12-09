using Microsoft.Identity.Client;
using System.Net.Http;
using System.Text.Json;

namespace FinancialTrackr.Services
{
    public class CurrencyServices
    {
        private readonly HttpClient _httpClient;
       
        public CurrencyServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency)
        {
            //Itt két API-t használok, ha az első failel van fallback
            baseCurrency = "HUF";
            string apiUrl = $"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@latest/v1/currencies/{baseCurrency.ToLower()}.json";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                //Sajnos egyik sem real time.
                apiUrl = $"https://api.frankfurter.app/latest?from={baseCurrency}";
                response = await _httpClient.GetAsync(apiUrl);
                var stream = await response.Content.ReadAsStreamAsync();
                var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

                if (json.TryGetProperty("rates", out JsonElement ratesElement))
                {
                    var rates = new Dictionary<string, decimal>();
                    foreach (var rate in ratesElement.EnumerateObject())
                    {
                        rates[rate.Name] = rate.Value.GetDecimal();
                    }
                    return rates;
                }

            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

                if (json.TryGetProperty(baseCurrency.ToLower(), out JsonElement ratesElement))
                {
                    var rates = new Dictionary<string, decimal>();
                    foreach (var rate in ratesElement.EnumerateObject())
                    {
                        rates[rate.Name] = rate.Value.GetDecimal();
                    }
                    return rates;
                }

            }
            return new Dictionary<string, decimal>();
        }
    }
}
