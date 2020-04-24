using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradesApi.Data.EntityFramework.Entities
{
    public class TradeEntity : BaseEntity
    {

        [ForeignKey(nameof(BidCurrency))]
        public int BidCurrencyId { get; set; }

        [ForeignKey(nameof(AskCurrency))]
        public int AskCurrencyId { get; set; }

        public virtual CurrencyEntity BidCurrency { get; set; }

        public virtual CurrencyEntity AskCurrency { get; set; }

        public decimal SoldAmount { get; set; }

        public decimal BoughtByClientAmount { get; set; }

        public decimal BoughtByUsAmount { get; set; }

        public DateTime Time { get; set; }

        public string ClientName { get; set; }
    }
}
