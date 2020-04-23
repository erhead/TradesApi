using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradesApi.BusinessLogic
{
    public class StaticCurrencyRatesProvider : ICurrencyRatesProvider
    {
        public Dictionary<Tuple<string, string>, decimal> Rates { get; private set; } =
            new Dictionary<Tuple<string, string>, decimal>
            {
                { new Tuple<string, string>("RUB", "USD"), 1m / 70m },
                { new Tuple<string, string>("USD", "GBP"), 0.6m }
            };

        public virtual Task<decimal> GetRateAsync(string baseCurrencyCode, string quoteCurrencyCode, DateTime date)
        {
            var pair = new Tuple<string, string>(baseCurrencyCode, quoteCurrencyCode);
            if (!Rates.ContainsKey(pair))
            {
                throw new InvalidOperationException($"There is no rate for pair '{pair.Item1}/{pair.Item2}'");
            }

            return Task.FromResult(Rates[pair]);
        }
    }
}
