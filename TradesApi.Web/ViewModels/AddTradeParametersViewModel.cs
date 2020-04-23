using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradesApi.Web.ViewModels
{
    public class AddTradeParametersViewModel
    {
        public string BidCurrencyCode { get; set; }

        public string AskCurrencyCode { get; set; }

        public decimal SoldAmount { get; set; }

        public DateTime Time { get; set; }

        public string ClientName { get; set; }
    }
}
