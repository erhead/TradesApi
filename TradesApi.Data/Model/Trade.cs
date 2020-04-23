using System;

namespace TradesApi.Data.Model
{
    public class Trade : BaseModel
    {
        public Currency BidCurrency { get; set; }

        public Currency AskCurrency { get; set; }

        public decimal SoldAmount { get; set; }

        public decimal BoughtByClientAmount { get; set; }

        public decimal BoughtByUsAmount { get; set; }

        public DateTime Time { get; set; }

        public string ClientName { get; set; }
    }
}
