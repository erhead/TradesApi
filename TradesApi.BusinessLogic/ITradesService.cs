using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradesApi.BusinessLogic.DataTransferObjects;

namespace TradesApi.BusinessLogic
{
    public interface ITradesService
    {
        Task AddTradeAsync(AddTradeParameters parameters);

        Task<TradeDto> GetTradeAsync(int id);

        Task<List<TradeDto>> GetTradesListAsync(int skip, int take);

        Task<List<DayProfitInGbpInfo>> GetProfitInGbpAsync(DateTime startDate, DateTime endDate);
    }
}
