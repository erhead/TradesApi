using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TradesApi.BusinessLogic;

namespace TradesApi.OpenExchangeRates
{
    public class OxrCurrencyRatesProvider : ICurrencyRatesProvider
    {
        private const string UsdCode = "USD";

        private const string DefaultOxrApiRoot = "https://openexchangerates.org/api";

        private Dictionary<int, List<Rate>> _usdRatesCache = new Dictionary<int, List<Rate>>();

        private HttpClient _httpClient;

        private int GetDateKey(int year, int month, int day)
        {
            return year * 500 + month * 40 + day;
        }

        public string OxrApiRoot { get; set; }

        public string ApplicationId { get; set; }

        public OxrCurrencyRatesProvider(HttpClient httpClient)
        {
            OxrApiRoot = DefaultOxrApiRoot;
            _httpClient = httpClient;
        }

        public OxrCurrencyRatesProvider(HttpClient httpClient, string applicationId) : this(httpClient)
        {
            ApplicationId = applicationId;
        }

        private async Task<List<Rate>> GetUsdRatesForDateAsync(DateTime date)
        {
            var dateKey = GetDateKey(date.Year, date.Month, date.Day);
            if (_usdRatesCache.ContainsKey(dateKey))
            {
                return _usdRatesCache[dateKey];
            }

            string dateString = date.ToString("yyyy-MM-dd");
            string url = $"{OxrApiRoot.TrimEnd('/')}/historical/{dateString}.json?app_id={ApplicationId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception e)
            {
                throw new HttpRequestFailedException(
                    $"OxrCurrencyRatesProvider: HTTP request failed. {e.Message}",
                    e);
            }

            if (response.IsSuccessStatusCode)
            {
                RatesResponse ratesResponse;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    JsonReader reader = new JsonTextReader(new StreamReader(responseStream));
                    ratesResponse = (new JsonSerializer()).Deserialize<RatesResponse>(reader);
                }

                var rates = ratesResponse.Rates
                    .Select(x => new Rate { CurrencyCode = x.Key, Value = x.Value })
                    .ToList();
                _usdRatesCache[dateKey] = rates;
                return rates;
            }
            else
            {
                throw new HttpRequestFailedException(
                    response.StatusCode,
                    $"OxrCurrencyRatesProvider: HTTP request failed. {response.ReasonPhrase}");
            }
        }

        private async Task<decimal> GetUsdRateForCurrencyAsync(string currencyCode, DateTime date)
        {
            var rates = await GetUsdRatesForDateAsync(date);
            var rate = rates.FirstOrDefault(x => x.CurrencyCode == currencyCode);
            if (rate != null)
            {
                return rate.Value;
            }
            else
            {
                throw new RateNotFoundException(
                    $"USD rate for currency '{currencyCode}' on {date.ToString("yyyy-MM-dd")} is not found");
            }

        }

        public async Task<decimal> GetRateAsync(string baseCurrencyCode, string quoteCurrencyCode, DateTime date)
        {
            decimal baseUsdRate = 1m;
            decimal quoteUsdRate = 1m;
            if (baseCurrencyCode != UsdCode)
            {
                baseUsdRate = await GetUsdRateForCurrencyAsync(baseCurrencyCode, date);
                if (baseUsdRate == 0)
                {
                    throw new RateNotFoundException(
                        $"USD rate for currency '{baseCurrencyCode}' on {date.ToString("yyyy-MM-dd")} is 0");
                }
            }
            if (quoteCurrencyCode != UsdCode)
            {
                quoteUsdRate = await GetUsdRateForCurrencyAsync(quoteCurrencyCode, date);
            }

            return quoteUsdRate / baseUsdRate;
        }
    }
}
