using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.Data.EntityFramework.Entities;
using TradesApi.Data.Model;

namespace TradesApi.Data.EntityFramework
{
    public abstract class EfRepository<TModel, TEntity> : IRepository<TModel>
        where TModel : BaseModel
        where TEntity : BaseEntity
    {
        protected TradesApiContext Context { get; set; }

        protected abstract TModel MapToModel(TEntity entity);

        protected abstract TEntity MapToEntity(TModel model);

        protected abstract DbSet<TEntity> GetDbSet();

        public EfRepository(TradesApiContext context)
        {
            Context = context;
        }

        public async Task CreateAsync(TModel item)
        {
            var set = GetDbSet();
            var entity = MapToEntity(item);
            await set.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var set = GetDbSet();
            var item = set.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                set.Remove(item);
                await Context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<TModel>> FindAsync(Func<TModel, bool> predicate)
        {
            var set = GetDbSet();
            var list = await set.ToListAsync();
            return list.Select(x => MapToModel(x)).Where(predicate);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var set = GetDbSet();
            var list = await set.ToListAsync();
            return list.Select(x => MapToModel(x)).ToList();
        }

        public async Task<TModel> GetAsync(int id)
        {
            var set = GetDbSet();
            return MapToModel(await set.FirstOrDefaultAsync(x => x.Id == id));
        }

        public IQueryable<TModel> GetQueryable()
        {
            var set = GetDbSet();
            var enumerable = set.ToList().Select(x => MapToModel(x));
            return enumerable.AsQueryable();
        }

        public async Task UpdateAsync(TModel item)
        {
            var set = GetDbSet();
            if (item != null)
            {
                set.Remove(set.FirstOrDefault(x => x.Id == item.Id));
                await set.AddAsync(MapToEntity(item));
                await Context.SaveChangesAsync();
            }
        }
    }
}
