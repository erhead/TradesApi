using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TradesApi.BusinessLogic.DataTransferObjects;
using TradesApi.Data;
using TradesApi.Data.InMemory;
using TradesApi.Data.Model;

namespace TradesApi.BusinessLogic.Tests
{
    [TestClass]
    public class TradesServiceTests
    {
        private TradesService CreateTradesService(
            ILogger logger = null,
            IRepository<Currency> currencyRepository = null,
            IRepository<Trade> tradesRepository = null,
            ICurrencyRatesProvider currencyRatesProvider = null,
            IConfigurationService configurationService = null)
        {
            logger = logger ?? new FakeLogger();
            currencyRepository = currencyRepository ?? new CurrencyInMemoryRepository();
            tradesRepository = tradesRepository ?? new TradesInMemoryRepository();
            currencyRatesProvider = currencyRatesProvider ?? new MockCurrencyRatesProvider();
            configurationService = configurationService ?? new MockConfigurationService();
            return new TradesService(logger, currencyRepository, tradesRepository, configurationService, currencyRatesProvider);
        }

        [TestMethod]
        public void AddTradeCorrectTest()
        {
            // Arrange.
            var currencyRatesProvider = new MockCurrencyRatesProvider();
            var configurationService = new MockConfigurationService();
            var tradesRepository = new TradesInMemoryRepository();
            currencyRatesProvider.Rates.Add(new Tuple<string, string>(Constants.Rub, Constants.Usd), 1m / 70m);
            configurationService.TotalEnrichmentPercent = 50m;
            var tradesService = CreateTradesService(
                tradesRepository: tradesRepository,
                currencyRatesProvider: currencyRatesProvider, 
                configurationService: configurationService);
            string clientName = "TestClient";

            // Act.
            var parameters = new AddTradeParameters
            {
                AskCurrencyCode = Constants.Usd,
                BidCurrencyCode = Constants.Rub,
                SoldAmount = 210,
                Time = DateTime.Now,
                ClientName = clientName
            };
            var result = tradesService.AddTradeAsync(parameters).Result;

            // Assert.
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(1, tradesRepository.GetQueryable().Count());
            var trade = tradesRepository.GetAllAsync().Result.First();
            Assert.AreEqual(Constants.Usd, trade.AskCurrency.Code);
            Assert.AreEqual(Constants.Rub, trade.BidCurrency.Code);
            Assert.AreEqual(2m, trade.BoughtByClientAmount);
            Assert.AreEqual(3m, trade.BoughtByUsAmount);
            Assert.AreEqual(clientName, trade.ClientName);
        }

        [TestMethod]
        public void AddTradeNonExistingCurrencyTest()
        {
            // Arrange.
            var tradesService = CreateTradesService();

            // Act.
            var parameters = new AddTradeParameters
            {
                AskCurrencyCode = Constants.Usd,
                BidCurrencyCode = "ABC",
                SoldAmount = 210,
                Time = DateTime.Now,
                ClientName = "TestClient"
            };
            var result = tradesService.AddTradeAsync(parameters).Result;

            // Assert.
            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void GetTradeCorrectTest()
        {
            // Arrange.
            var currencyRepository = new CurrencyInMemoryRepository();
            int tradeId = 1;
            Currency askCurrency = currencyRepository.GetQueryable().First(x => x.Code == Constants.Usd);
            Currency bidCurrency = currencyRepository.GetQueryable().First(x => x.Code == Constants.Rub);
            decimal soldAmount = 210m;
            decimal boughtByClientAmount = 2m;
            decimal boughtByUsAmount = 3m;
            DateTime time = new DateTime(2020, 1, 1);
            string clientName = "TestClient";
            var tradesRepository = new TradesInMemoryRepository(new List<Trade> 
            {
                new Trade
                {
                    Id = tradeId,
                    AskCurrency = askCurrency,
                    BidCurrency = bidCurrency,
                    SoldAmount = soldAmount,
                    BoughtByClientAmount = boughtByClientAmount,
                    BoughtByUsAmount = boughtByUsAmount,
                    Time = time,
                    ClientName = clientName
                }
            });
            var tradesService = CreateTradesService(tradesRepository: tradesRepository);

            // Act.
            var result = tradesService.GetTradeAsync(new GetTradeParameters { TradeId = tradeId }).Result;

            // Assert.
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(tradeId, result.Trade.Id);
            Assert.AreEqual(askCurrency.Code, result.Trade.AskCurrencyCode);
            Assert.AreEqual(bidCurrency.Code, result.Trade.BidCurrencyCode);
            Assert.AreEqual(soldAmount, result.Trade.SoldAmount);
            Assert.AreEqual(boughtByClientAmount, result.Trade.BoughtAmount);
            Assert.AreEqual(boughtByClientAmount / soldAmount, result.Trade.ClientRate);
            Assert.AreEqual(boughtByUsAmount / soldAmount, result.Trade.BrokerRate);
            Assert.AreEqual(time, result.Trade.Time);
            Assert.AreEqual(clientName, result.Trade.ClientName);
        }

        [TestMethod]
        public void GetTradesListCorrectTest()
        {
            // Arrange.
            var currencyRepository = new CurrencyInMemoryRepository();
            Currency usd = currencyRepository.GetQueryable().First(x => x.Code == Constants.Usd);
            Currency rub = currencyRepository.GetQueryable().First(x => x.Code == Constants.Rub);
            var trades = new List<Trade>();
            for (var i = 0; i < 4; i++)
            {
                trades.Add(new Trade
                {
                    Id = i,
                    AskCurrency = usd,
                    BidCurrency = rub,
                    SoldAmount = 210m,
                    BoughtByClientAmount = 2m,
                    BoughtByUsAmount = 3m,
                    Time = DateTime.Now,
                    ClientName = "TestClient"
                });
            }
            var tradesRepository = new TradesInMemoryRepository(trades);
            var tradesService = CreateTradesService(tradesRepository: tradesRepository);

            // Act.
            var parameters = new GetTradesListParameters
            {
                Skip = 1,
                Take = 2
            };
            var result = tradesService.GetTradesListAsync(parameters).Result;

            // Assert.
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(2, result.Trades.Count);
        }

        [TestMethod]
        public void GetProficInGbpCorrect()
        {
            // Arrange.
            var currencyRatesProvider = new MockCurrencyRatesProvider();
            currencyRatesProvider.Rates.Add(new Tuple<string, string>(Constants.Usd, Constants.Gbp), 0.6m);
            var currencyRepository = new CurrencyInMemoryRepository();
            Currency usd = currencyRepository.GetQueryable().First(x => x.Code == Constants.Usd);
            Currency rub = currencyRepository.GetQueryable().First(x => x.Code == Constants.Rub);
            var trades = new List<Trade>();
            for (var i = 0; i < 4; i++)
            {
                trades.Add(new Trade
                {
                    Id = i,
                    AskCurrency = usd,
                    BidCurrency = rub,
                    SoldAmount = 210m,
                    BoughtByClientAmount = 2m,
                    BoughtByUsAmount = 3m,
                    Time = new DateTime(2020, 1, 1 + i),
                    ClientName = "TestClient"
                });
            }
            var tradesRepository = new TradesInMemoryRepository(trades);
            var tradesService = CreateTradesService(
                tradesRepository: tradesRepository,
                currencyRatesProvider: currencyRatesProvider);

            // Act.
            var parameters = new GetProfitInGbpReportParameters
            {
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 5)
            };
            var result = tradesService.GetProfitInGbpAsync(parameters).Result;

            // Assert.
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(4, result.ProfitData.Count);
            Assert.AreEqual(new DateTime(2020, 1, 2), result.ProfitData[0].Date);
            Assert.AreEqual(0.6m, result.ProfitData[0].Sum);
            Assert.AreEqual(new DateTime(2020, 1, 5), result.ProfitData[3].Date);
            Assert.AreEqual(0m, result.ProfitData[3].Sum);
        }
    }
}
