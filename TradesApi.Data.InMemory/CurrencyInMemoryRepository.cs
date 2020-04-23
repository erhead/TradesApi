using System.Collections.Generic;
using TradesApi.Data.Model;

namespace TradesApi.Data.InMemory
{
    public class CurrencyInMemoryRepository : InMemoryRepository<Currency>
    {
        private static List<Currency> DefaultCurrencyList = new List<Currency>
        {
            new Currency { Id = 0, Code = "USD" },
            new Currency { Id = 1, Code = "EUR" },
            new Currency { Id = 2, Code = "GBP" },
            new Currency { Id = 3, Code = "RUB" },
        };

        public CurrencyInMemoryRepository() : base(DefaultCurrencyList)
        {
        }

        public CurrencyInMemoryRepository(IEnumerable<Currency> currencies) : base(currencies)
        {
        }
    }
}
