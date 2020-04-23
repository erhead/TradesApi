namespace TradesApi.BusinessLogic.Tests
{
    internal class MockConfigurationService : IConfigurationService
    {
        public decimal TotalEnrichmentPercent { get; set; } = 50m;

        public decimal GetTotalEnrichmentPercent()
        {
            return TotalEnrichmentPercent;
        }
    }
}
