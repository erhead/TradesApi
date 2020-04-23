using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.Data.Model;

namespace TradesApi.Data
{
    public interface IRepository<T> where T : BaseModel
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetAsync(int id);

        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

        IQueryable<T> GetQueryable();

        Task CreateAsync(T item);

        Task UpdateAsync(T item);

        Task<bool> DeleteAsync(int id);
    }
}
