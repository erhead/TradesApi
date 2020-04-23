
using System;
using System.Collections.Generic;
using System.Text;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class GetTradeResult : BusinessActionResult
    {
        public TradeDto Trade { get; set; }
    }
}
