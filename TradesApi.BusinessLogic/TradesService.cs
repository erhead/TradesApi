using System;
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

        public Task<AddTradeResult> AddTradeAsync(AddTradeParameters parameters)
        {
            throw new NotImplementedException();
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
