using System;
using System.Threading.Tasks;

namespace TradesApi.BusinessLogic
{
    public interface ICurrencyRatesProvider
    {
        Task<decimal> GetRateAsync(string baseCurrencyCode, string quoteCurrencyCode, DateTime date);
    }
}
