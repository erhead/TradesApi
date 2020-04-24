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
            IRepository<Currency> currencyRepository,
            IRepository<Trade> tradesRepository,
            IConfigurationService configurationService,
            ICurrencyRatesProvider currencyRatesProvider)
        {
            _currencyRepository = currencyRepository;
            _tradesRepository = tradesRepository;
            _configurationService = configurationService;
            _currencyRatesProvider = currencyRatesProvider;
        }

        public async Task AddTradeAsync(AddTradeParameters parameters)
        {
            if (parameters.SoldAmount <= 0)
            {
                throw new IncorrectParametersException("An incorrect sold amount specified");
            }

            Currency askCurrency = (await _currencyRepository
                .FindAsync(x => x.Code == parameters.AskCurrencyCode))
                .FirstOrDefault();
            Currency bidCurrency = (await _currencyRepository
                .FindAsync(x => x.Code == parameters.BidCurrencyCode))
                .FirstOrDefault();

            if (askCurrency == null || bidCurrency == null)
            {
                throw new IncorrectParametersException("TradesService: An incorrect currency code specified");
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
        }

        public async Task<List<DayProfitInGbpInfo>> GetProfitInGbpAsync(DateTime startDate, DateTime endDate)
        {
            // Make sure to use dates only.
            var alignedStartDate = new DateTime(
                startDate.Year,
                startDate.Month,
                startDate.Day);
            var alignedEndDate = new DateTime(
                endDate.Year,
                endDate.Month,
                endDate.Day, 23, 59, 59, 999);
            var tradeModels = await _tradesRepository.FindAsync(
                x => x.Time >= alignedStartDate && x.Time <= alignedEndDate);
            var profits = new Dictionary<DateTime, decimal>();
            var c = alignedStartDate;
            while (c <= alignedEndDate)
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
            return profits
                    .Select(x => new DayProfitInGbpInfo { Date = x.Key, Sum = x.Value })
                    .OrderBy(x => x.Date)
                    .ToList();
        }

        public async Task<TradeDto> GetTradeAsync(int id)
        {
            var tradeModel = await _tradesRepository.GetAsync(id);
            if (tradeModel == null)
            {
                throw new ObjectNotFoundException("TradesService: An invalid trade ID specified");
            }
            return new TradeDto(tradeModel);
        }

        public async Task<List<TradeDto>> GetTradesListAsync(int skip, int take)
        {
            if (skip < 0 || take < 0)
            {
                throw new IncorrectParametersException("TradesService: invalid skip/take parameters specified");
            }
            var tradeModels = _tradesRepository
                .GetQueryable()
                .OrderBy(x => x.Id)
                .Skip(skip);
            if (take > 0)
            {
                tradeModels = tradeModels.Take(take);
            }
            List<Trade> tradesList = await GetListFromIQueryableAsync(tradeModels);
            return tradesList.Select(x => new TradeDto(x)).ToList();
        }
    }
}
