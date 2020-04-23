using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.BusinessLogic;
using TradesApi.BusinessLogic.DataTransferObjects;
using TradesApi.Web.ViewModels;

namespace TradesApi.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly TradesApi.Data.ILogger _logger;

        private readonly ITradesService _tradesService;

        public TradesController(
            ILogger<TradesController> logger,
            ITradesService tradesService)
        {
            _logger = new Logger(logger);
            _tradesService = tradesService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _tradesService.GetTradeAsync(new GetTradeParameters { TradeId = id });
            if (result.Successful)
            {
                return new JsonResult(new TradeViewModel(result.Trade));
            }
            else
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(new ErrorViewModel { Message = result.ErrorMessage });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(AddTradeParametersViewModel parameters)
        {
            var result = await _tradesService.AddTradeAsync(new AddTradeParameters
            {
                AskCurrencyCode = parameters.AskCurrencyCode,
                BidCurrencyCode = parameters.BidCurrencyCode,
                ClientName = parameters.ClientName,
                SoldAmount = parameters.SoldAmount,
                Time = parameters.Time
            });
            if (result.Successful)
            {
                return new EmptyResult();
            }
            else
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(new ErrorViewModel { Message = result.ErrorMessage });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetList(int skip = 0, int take = 0)
        {
            var result = await _tradesService.GetTradesListAsync(
                new GetTradesListParameters { Skip = skip, Take = take });
            if (result.Successful)
            {
                return new JsonResult(result.Trades);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(new ErrorViewModel { Message = result.ErrorMessage });
            }
        }

        [HttpGet]
        [Route("gpb-profit-report")]
        public async Task<ActionResult> GetProfitInGpbReport(DateTime startDate, DateTime endDate)
        {
            var result = await _tradesService.GetProfitInGbpAsync(
                new GetProfitInGbpReportParameters { StartDate = startDate, EndDate = endDate });
            if (result.Successful)
            {
                return new JsonResult(result.ProfitData.Select(x => new ProfitInGbpReportViewModel(x)));
            }
            else
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(new ErrorViewModel { Message = result.ErrorMessage });
            }
        }
    }
}
