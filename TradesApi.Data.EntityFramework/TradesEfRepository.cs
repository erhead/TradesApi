using Microsoft.EntityFrameworkCore;
using TradesApi.Data.EntityFramework.Entities;
using TradesApi.Data.Model;

namespace TradesApi.Data.EntityFramework
{
    public class TradesEfRepository : EfRepository<Trade, TradeEntity>
    {
        protected override DbSet<TradeEntity> GetDbSet()
        {
            return Context.Trades;
        }

        protected override TradeEntity MapToEntity(Trade model)
        {
            return new TradeEntity
            {
                Id = model.Id,
                AskCurrencyId = model.AskCurrency.Id,
                BidCurrencyId = model.BidCurrency.Id,
                BoughtByClientAmount = model.BoughtByClientAmount,
                BoughtByUsAmount = model.BoughtByUsAmount,
                ClientName = model.ClientName,
                SoldAmount = model.SoldAmount,
                Time = model.Time
            };
        }

        protected override Trade MapToModel(TradeEntity entity)
        {
            return new Trade
            {
                Id = entity.Id,
                AskCurrency = new Currency { Id = entity.AskCurrency.Id, Code = entity.AskCurrency.Code },
                BidCurrency = new Currency { Id = entity.BidCurrency.Id, Code = entity.BidCurrency.Code },
                BoughtByClientAmount = entity.BoughtByClientAmount,
                BoughtByUsAmount = entity.BoughtByUsAmount,
                ClientName = entity.ClientName,
                SoldAmount = entity.SoldAmount,
                Time = entity.Time
            };
        }

        public TradesEfRepository(TradesApiContext context) : base(context)
        {
        }
    }
}
