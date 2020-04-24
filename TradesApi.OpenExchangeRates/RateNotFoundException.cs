using System;

namespace TradesApi.OpenExchangeRates
{
    public class RateNotFoundException : Exception
    {
        public RateNotFoundException(): base()
        {
        }

        public RateNotFoundException(string message) : base(message)
        {
        }
    }
}
