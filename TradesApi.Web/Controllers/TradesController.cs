using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
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
        [SwaggerOperation("Get a trade by ID", "Get a specific trade instance by ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(TradeViewModel))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(ErrorViewModel))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorViewModel))]
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
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return new JsonResult(new ErrorViewModel { Message = e.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation("Add a trade", "Add a trade by specified parameters")]
        [SwaggerResponse(StatusCodes.Status200OK, "OK")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not found", typeof(ErrorViewModel))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorViewModel))]
        public async Task<ActionResult> Post([SwaggerRequestBody("Creation parameters")] AddTradeParametersViewModel parameters)
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
        [SwaggerOperation("Get multiple trades", "Get trades using skip and take parameters")]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(List<TradeViewModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(ErrorViewModel))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorViewModel))]
        public async Task<ActionResult> GetList(
            [SwaggerParameter("How many leading trades to skip (no value to cancel skipping)")] int skip = 0,
            [SwaggerParameter("How many trades to take (no value to get all trades)")] int take = 0)
        {
            try
            {
                var result = await _tradesService.GetTradesListAsync(skip, take);
                return new JsonResult(result.Select(x => new TradeViewModel(x)));
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
        [SwaggerOperation("Get a GBP profit report", "Get a report containing records for every day from " +
            "the specified range. Each record contains a per-day profit in GBP")]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(List<ProfitInGbpReportViewModel>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorViewModel))]
        public async Task<ActionResult> GetProfitInGpbReport(
            [SwaggerParameter("An initial range date ('YYYY-MM-DD')")] DateTime startDate,
            [SwaggerParameter("A final range date ('YYYY-MM-DD')")] DateTime endDate)
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
