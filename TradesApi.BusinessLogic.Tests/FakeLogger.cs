using TradesApi.Data;

namespace TradesApi.BusinessLogic.Tests
{
    internal class FakeLogger : ILogger
    {
        public void LogError(string message, string stackTrace)
        {
        }

        public void LogInfo(string message)
        {
        }
    }
}
