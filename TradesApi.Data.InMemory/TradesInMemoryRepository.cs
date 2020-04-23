using System.Collections.Generic;
using TradesApi.Data.Model;

namespace TradesApi.Data.InMemory
{
    public class TradesInMemoryRepository : InMemoryRepository<Trade>
    {
        public TradesInMemoryRepository()
        { 
        }

        public TradesInMemoryRepository(IEnumerable<Trade> trades) : base(trades)
        {
        }
    }
}
