﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TradesApi.BusinessLogic.DataTransferObjects;
using TradesApi.Data;
using TradesApi.Data.Model;

namespace TradesApi.BusinessLogic
{
    public class TradesService : ITradesService
    {
        private ILogger _logger;

        private IRepository<Currency> _currencyRepository;

        private IRepository<Trade> _tradesRepository;

        private IConfigurationService _configurationService;

        private ICurrencyRatesProvider _currencyRatesProvider;

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
                    string message = "An incorrect currency code specified";
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

        public Task<GetProfitInGbpReportResult> GetProfitInGbpAsync(GetProfitInGbpReportParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<GetTradeResult> GetTradeAsync(GetTradeParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<GetTradesListResult> GetTradesListAsync(GetTradesListParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
