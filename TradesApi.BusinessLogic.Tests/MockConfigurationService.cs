using System.Threading.Tasks;

namespace TradesApi.BusinessLogic.Tests
{
    internal class MockConfigurationService : IConfigurationService
    {
        public decimal TotalEnrichmentPercent { get; set; } = 50m;

        public Task<decimal> GetTotalEnrichmentPercentAsync()
        {
            return Task.FromResult(TotalEnrichmentPercent);
        }
    }
}
