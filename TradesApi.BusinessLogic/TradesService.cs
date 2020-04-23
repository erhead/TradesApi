using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.BusinessLogic.DataTransferObjects;
using TradesApi.Data;
using TradesApi.Data.Model;

namespace TradesApi.BusinessLogic
{
    public class TradesService : ITradesService
    {
        private const string GbpCode = "GBP";

        private ILogger _logger;

        private IRepository<Currency> _currencyRepository;

        private IRepository<Trade> _tradesRepository;

        private IConfigurationService _configurationService;

        private ICurrencyRatesProvider _currencyRatesProvider;

        private async Task<List<T>> GetListFromIQueryableAsync<T>(IQueryable<T> queryable)
        {
            if (queryable is IAsyncEnumerable<T>)
            {
                return await queryable.ToListAsync();
            }
            else
            {
                return queryable.ToList();
            }
        }

        public TradesService(
            ILogger logger,
            IRepository<Currency> currencyRepository,
            IRepository<Trade> tradesRepository,
            IConfigurationService configurationService,
            ICurrencyRatesProvider currencyRatesProvider)
        {
            _logger = logger;
            _currencyRepository = currencyRepository;
            _tradesRepository = tradesRepository;
            _configurationService = configurationService;
            _currencyRatesProvider = currencyRatesProvider;
        }

        public async Task<AddTradeResult> AddTradeAsync(AddTradeParameters parameters)
        {
            try
            {
                if (parameters.SoldAmount <= 0)
                {
                    string message = "An incorrect sold amount specified";
                    _logger.LogError($"TradesService: {message}", null);
                    return new AddTradeResult
                    {
                        Successful = false,
                        ErrorMessage = message
                    };
                }

                Currency askCurrency = (await _currencyRepository
                    .FindAsync(x => x.Code == parameters.AskCurrencyCode))
                    .FirstOrDefault();
                Currency bidCurrency = (await _currencyRepository
                    .FindAsync(x => x.Code == parameters.BidCurrencyCode))
                    .FirstOrDefault();

                if (askCurrency == null || bidCurrency == null)
                {
                    string message = "TradesService: An incorrect currency code specified";
                    _logger.LogError($"TradesService: {message}", null);
                    return new AddTradeResult
                    {
                        Successful = false,
                        ErrorMessage = message
                    };
                }

                decimal brokerRate = await _currencyRatesProvider.GetRateAsync(
                        parameters.BidCurrencyCode,
                        parameters.AskCurrencyCode,
                        parameters.Time);
                decimal totalEnrichmentPercent = await _configurationService.GetTotalEnrichmentPercentAsync();
                decimal rateEnrichmentCoefficient = 1m / (1m + totalEnrichmentPercent / 100m);
                decimal clientRate = brokerRate * rateEnrichmentCoefficient;

                var tradeModel = new Trade
                {
                    AskCurrency = askCurrency,
                    BidCurrency = bidCurrency,
                    SoldAmount = parameters.SoldAmount,
                    BoughtByUsAmount = decimal.Round(parameters.SoldAmount * brokerRate, 2),
                    BoughtByClientAmount = decimal.Round(parameters.SoldAmount * clientRate, 2),
                    ClientName = parameters.ClientName,
                    Time = parameters.Time
                };
                await _tradesRepository.CreateAsync(tradeModel);
                return new AddTradeResult
                {
                    Successful = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new AddTradeResult
                {
                    Successful = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetProfitInGbpReportResult> GetProfitInGbpAsync(GetProfitInGbpReportParameters parameters)
        {
            try
            {
                // Make sure to use dates only.
                var startDate = new DateTime(
                    parameters.StartDate.Year,
                    parameters.StartDate.Month,
                    parameters.StartDate.Day);
                var endDate = new DateTime(
                    parameters.EndDate.Year,
                    parameters.EndDate.Month,
                    parameters.EndDate.Day, 23, 59, 59, 999);
                var tradeModels = await _tradesRepository.FindAsync(
                    x => x.Time >= startDate && x.Time <= endDate);
                var profits = new Dictionary<DateTime, decimal>();
                var c = startDate;
                while (c <= endDate)
                {
                    profits.Add(c, 0m);
                    c = c.AddDays(1);
                }
                foreach (var trade in tradeModels)
                {
                    // Disregard time.
                    var date = new DateTime(trade.Time.Year, trade.Time.Month, trade.Time.Day);
                    if (!profits.ContainsKey(date))
                    {
                        profits.Add(date, 0m);
                    }

                    var rate = await _currencyRatesProvider.GetRateAsync(trade.AskCurrency.Code, GbpCode, date);
                    profits[date] = profits[date] + (trade.BoughtByUsAmount - trade.BoughtByClientAmount) * rate;
                }
                return new GetProfitInGbpReportResult
                {
                    Successful = true,
                    ProfitData = profits
                        .Select(x => new DayProfitInGbpInfo { Date = x.Key, Sum = x.Value })
                        .OrderBy(x => x.Date)
                        .ToList()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new GetProfitInGbpReportResult
                {
                    Successful = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetTradeResult> GetTradeAsync(GetTradeParameters parameters)
        {
            try
            {
                var tradeModel = await _tradesRepository.GetAsync(parameters.TradeId);
                if (tradeModel == null)
                {
                    string message = "TradesService: An invalid trade ID specified";
                    _logger.LogError(message, null);
                    return new GetTradeResult
                    {
                        Successful = false,
                        ErrorMessage = message
                    };
                }
                return new GetTradeResult
                {
                    Successful = true,
                    Trade = new TradeDto(tradeModel)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new GetTradeResult
                {
                    Successful = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetTradesListResult> GetTradesListAsync(GetTradesListParameters parameters)
        {
            try
            {
                if (parameters.Skip < 0 || parameters.Take < 0)
                {
                    string message = "TradesService: invalid skip/take parameters specified";
                    _logger.LogError(message, null);
                    return new GetTradesListResult
                    {
                        Successful = false,
                        ErrorMessage = message
                    };
                }
                var tradeModels = _tradesRepository
                    .GetQueryable()
                    .OrderBy(x => x.Id)
                    .Skip(parameters.Skip);
                if (parameters.Take > 0)
                {
                    tradeModels = tradeModels.Take(parameters.Take);
                }
                List<Trade> tradesList = await GetListFromIQueryableAsync(tradeModels);
                return new GetTradesListResult
                {
                    Successful = true,
                    Trades = tradesList.Select(x => new TradeDto(x)).ToList()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new GetTradesListResult
                {
                    Successful = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
