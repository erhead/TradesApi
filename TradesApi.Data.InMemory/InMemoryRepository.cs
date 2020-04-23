using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.Data.Model;

namespace TradesApi.Data.InMemory
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseModel
    {
        private Dictionary<int, T> _storage = new Dictionary<int, T>();

        public InMemoryRepository()
        {
        }

        public InMemoryRepository(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _storage.Add(item.Id, item);
            }
        }

        public virtual Task CreateAsync(T item)
        {
            if (item.Id == default(int))
            {
                if (_storage.Any())
                {
                    item.Id = _storage.Keys.Max() + 1;
                }
                else
                {
                    item.Id = 1;
                }
            }

            if (_storage.ContainsKey(item.Id))
            {
                throw new InvalidOperationException($"The item with id = \"{item.Id}\" already exists.");
            }

            _storage.Add(item.Id, item);
            return Task.CompletedTask;
        }

        public virtual Task<bool> DeleteAsync(int id)
        {
            if (!_storage.ContainsKey(id))
            {
                return Task.FromResult(false);
            }

            _storage.Remove(id);
            return Task.FromResult(true);
        }

        public virtual Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            return Task.FromResult(_storage.Values.Where(predicate));
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _storage.Values.AsQueryable();
        }

        public virtual Task<T> GetAsync(int id)
        {
            T result = null;
            if (_storage.ContainsKey(id))
            {
                result = _storage[id];
            }

            return Task.FromResult(result);
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_storage.Values.AsEnumerable());
        }

        public virtual Task UpdateAsync(T item)
        {
            if (!_storage.ContainsKey(item.Id))
            {
                throw new InvalidCastException($"The item with id = \"{item.Id}\" does not exist.");
            }

            _storage[item.Id] = item;
            return Task.CompletedTask;
        }
    }
}
