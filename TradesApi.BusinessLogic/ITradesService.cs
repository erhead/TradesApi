using System.Threading.Tasks;
using TradesApi.BusinessLogic.DataTransferObjects;

namespace TradesApi.BusinessLogic
{
    public interface ITradesService
    {
        Task<AddTradeResult> AddTradeAsync(AddTradeParameters parameters);

        Task<GetTradeResult> GetTradeAsync(GetTradeParameters parameters);

        Task<GetTradesListResult> GetTradesListAsync(GetTradesListParameters parameters);

        Task<GetProfitInGbpReportResult> GetProfitInGbpAsync(GetProfitInGbpReportParameters parameters);
    }
}
