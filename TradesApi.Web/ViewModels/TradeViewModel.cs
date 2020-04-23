using System;
using TradesApi.BusinessLogic.DataTransferObjects;

namespace TradesApi.Web.ViewModels
{
    public class TradeViewModel
    {
        public TradeViewModel()
        {
        }

        public TradeViewModel(TradeDto source)
        {
            Id = source.Id;
            AskCurrencyCode = source.AskCurrencyCode;
            BidCurrencyCode = source.BidCurrencyCode;
            SoldAmount = source.SoldAmount;
            BoughtAmount = source.BoughtAmount;
            BrokerRate = source.BrokerRate;
            ClientRate = source.ClientRate;
            ClientName = source.ClientName;
            Time = source.Time;
        }

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
