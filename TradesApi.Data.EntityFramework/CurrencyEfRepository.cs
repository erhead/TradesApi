using Microsoft.EntityFrameworkCore;
using TradesApi.Data.EntityFramework.Entities;
using TradesApi.Data.Model;

namespace TradesApi.Data.EntityFramework
{
    public class CurrencyEfRepository : EfRepository<Currency, CurrencyEntity>
    {
        protected override DbSet<CurrencyEntity> GetDbSet()
        {
            return Context.Currencies;
        }

        protected override CurrencyEntity MapToEntity(Currency model)
        {
            return new CurrencyEntity
            {
                Id = model.Id,
                Code = model.Code
            };
        }

        protected override Currency MapToModel(CurrencyEntity entity)
        {
            return new Currency
            {
                Id = entity.Id,
                Code = entity.Code
            };
        }

        public CurrencyEfRepository(TradesApiContext context) : base(context)
        {
        }
    }
}
