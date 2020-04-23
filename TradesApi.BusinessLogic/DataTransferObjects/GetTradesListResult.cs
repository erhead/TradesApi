using System.Collections.Generic;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class GetTradesListResult : BusinessActionResult
    {
        public List<TradeDto> Trades { get; set; }
    }
}
