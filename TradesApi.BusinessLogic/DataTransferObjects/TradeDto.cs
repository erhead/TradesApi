using System;
using TradesApi.Data.Model;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class TradeDto
    {
        public TradeDto()
        {
        }

        public TradeDto(Trade model)
        {
            Id = model.Id;
            AskCurrencyCode = model.AskCurrency.Code;
            BidCurrencyCode = model.BidCurrency.Code;
            SoldAmount = model.SoldAmount;
            BoughtAmount = model.BoughtByClientAmount;
            BrokerRate = model.BoughtByUsAmount / model.SoldAmount;
            ClientRate = model.BoughtByClientAmount / model.SoldAmount;
            ClientName = model.ClientName;
            Time = model.Time;
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
