using System.Threading.Tasks;

namespace TradesApi.BusinessLogic
{
    public class StaticConfigurationService : IConfigurationService
    {
        public decimal TotalEnrichmentPercent { get; set; } = 50m;

        public Task<decimal> GetTotalEnrichmentPercentAsync()
        {
            return Task.FromResult(TotalEnrichmentPercent);
        }
    }
}
