using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TradesApi.Data;
using TradesApi.Data.Model;

namespace TradesApi.OpenExchangeRates
{
    public class OxrCurrencyRepository : IRepository<Currency>
    {
        private const string DefaultOxrApiRoot = "https://openexchangerates.org/api";

        private Dictionary<int, Currency> _currencyCache = null;

        private HttpClient _httpClient;

        private async Task LoadCurrencies()
        {
            string url = $"{OxrApiRoot.TrimEnd('/')}/currencies.json";
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
                Dictionary<string, string> currenciesResponse;
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    JsonReader reader = new JsonTextReader(new StreamReader(responseStream));
                    currenciesResponse = (new JsonSerializer()).Deserialize<Dictionary<string, string>>(reader);
                }

                var currencies = currenciesResponse.Keys.ToList();
                currencies.Sort();
                var curId = 1;
                _currencyCache = new Dictionary<int, Currency>();
                foreach (var code in currencies)
                {
                    _currencyCache.Add(curId, new Currency 
                    {
                        Id = curId,
                        Code = code
                    });
                    curId++;
                }
            }
            else
            {
                throw new HttpRequestFailedException(
                    response.StatusCode,
                    $"OxrCurrencyRatesProvider: HTTP request failed. {response.ReasonPhrase}");
            }
        }

        public OxrCurrencyRepository(HttpClient httpClient)
        {
            OxrApiRoot = DefaultOxrApiRoot;
            _httpClient = httpClient;
        }

        public string OxrApiRoot { get; set; }

        public async Task CreateAsync(Currency item)
        { 
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return false;
        }

        public async Task<IEnumerable<Currency>> FindAsync(Func<Currency, bool> predicate)
        {
            if (_currencyCache == null)
            {
                await LoadCurrencies();
            }

            return _currencyCache.Values.Where(predicate);
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            if (_currencyCache == null)
            {
                await LoadCurrencies();
            }

            return _currencyCache.Values;
        }

        public async Task<Currency> GetAsync(int id)
        {
            if (_currencyCache == null)
            {
                await LoadCurrencies();
            }

            if (_currencyCache.ContainsKey(id))
            {
                return _currencyCache[id];
            } 
            else
            {
                return null;
            }
        }

        public IQueryable<Currency> GetQueryable()
        {
            if (_currencyCache == null)
            {
                LoadCurrencies().Wait();
            }

            return _currencyCache.Values.AsQueryable();
        }

        public async Task UpdateAsync(Currency item)
        {
        }
    }
}
