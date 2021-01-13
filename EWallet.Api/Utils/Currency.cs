using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EWallet.Api.Utils
{
    public class Currency
    {

        public bool Success { get; set; }
        public long TimeStamp { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }

        public static async Task<decimal> ConvertCurrency(string from, string to, decimal amount)
        {

            var _client = new HttpClient();
            var url = @"http://data.fixer.io/api/latest?access_key=ece4431f8bf792e29b44fa6e16987f78";
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var responseModel = JsonSerializer.Deserialize<Currency>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var currencies = responseModel.Rates.Keys;
            var currencyToBase = amount / responseModel.Rates[from];
            var convertedCurrency = responseModel.Rates[to] * currencyToBase;
            return convertedCurrency;
        }
    }

}
