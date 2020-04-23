using System.Threading.Tasks;

namespace TradesApi.BusinessLogic
{
    public interface IConfigurationService
    {
        Task<decimal> GetTotalEnrichmentPercentAsync();
    }
}
