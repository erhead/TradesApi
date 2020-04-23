using TradesApi.BusinessLogic.DataTransferObjects;

namespace TradesApi.Web.ViewModels
{
    public class ProfitInGbpReportViewModel
    {
        public ProfitInGbpReportViewModel()
        { 
        }

        public ProfitInGbpReportViewModel(DayProfitInGbpInfo source)
        {
            Date = source.Date.ToString("yyyy-MM-dd");
            Profit = source.Sum;
        }

        public string Date { get; set; }

        public decimal Profit { get; set; }
    }
}
