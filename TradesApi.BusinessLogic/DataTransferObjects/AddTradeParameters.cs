using System;
using System.Collections.Generic;
using System.Text;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class AddTradeParameters
    {
        public string BidCurrencyCode { get; set; }

        public string AskCurrencyCode { get; set; }

        public decimal SoldAmount { get; set; }

        public DateTime Time { get;set; }

        public string ClientName { get; set; }
    }
}
