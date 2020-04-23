using TradesApi.Data;

namespace TradesApi.BusinessLogic
{
    public class StubLogger : ILogger
    {
        public void LogError(string message, string stackTrace)
        { 
        }

        public void LogInfo(string message)
        {
        }
    }
}
