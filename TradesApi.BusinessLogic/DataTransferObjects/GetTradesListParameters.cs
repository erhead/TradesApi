using System;
using System.Collections.Generic;
using System.Text;

namespace TradesApi.BusinessLogic.DataTransferObjects
{
    public class GetTradesListParameters
    {
        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
