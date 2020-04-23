using System.Collections.Generic;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class GetProfitInGbpReportResult : BusinessActionResult
    {
        public List<DayProfitInGbpInfo> ProfitData { get; set; }
    }
}
