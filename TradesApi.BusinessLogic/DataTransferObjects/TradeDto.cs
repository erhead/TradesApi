using System;
using System.Collections.Generic;
using System.Text;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class TradeDto
    {
        public int Id { get; set; }

        public string BidCurrencyCode { get; set; }

        public string AskCurrencyCode { get; set; }

        public decimal SoldAmount { get; set; }

        public decimal BoughtAmount { get; set; }

        public decimal ClientRate { get; set; }

        public decimal BrokerRate { get; set; }

        public DateTime Time { get; set; }

        public string ClientName { get; set; }
    }
}
