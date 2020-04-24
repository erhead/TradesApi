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
        private readonly ITradesService _tradesService;

        public TradesController(
            ITradesService tradesService)
        {
            _tradesService = tradesService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var result = await _tradesService.GetTradeAsync(id);
                return new JsonResult(new TradeViewModel(result));
            }
            catch (ObjectNotFoundException e)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
            catch (IncorrectParametersException e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(AddTradeParametersViewModel parameters)
        {
            try
            {
                await _tradesService.AddTradeAsync(new AddTradeParameters
                {
                    AskCurrencyCode = parameters.AskCurrencyCode,
                    BidCurrencyCode = parameters.BidCurrencyCode,
                    ClientName = parameters.ClientName,
                    SoldAmount = parameters.SoldAmount,
                    Time = parameters.Time
                });
                return new EmptyResult();
            }
            catch (IncorrectParametersException e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetList(int skip = 0, int take = 0)
        {
            try
            {
                var result = await _tradesService.GetTradesListAsync(skip, take);
                return new JsonResult(result);
            }
            catch (IncorrectParametersException e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("GbpProfitReport")]
        public async Task<ActionResult> GetProfitInGpbReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _tradesService.GetProfitInGbpAsync(startDate, endDate);
                return new JsonResult(result.Select(x => new ProfitInGbpReportViewModel(x)));
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
        }
    }
}
