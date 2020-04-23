using Microsoft.Extensions.Logging;

namespace TradesApi.Web
{
    public class Logger : TradesApi.Data.ILogger
    {
        private Microsoft.Extensions.Logging.ILogger _logger;

        public Logger(Microsoft.Extensions.Logging.ILogger wrappedLogger)
        {
            _logger = wrappedLogger;
        }

        public void LogError(string message, string stackTrace)
        {
            _logger.LogError(string.IsNullOrEmpty(stackTrace) ? message : string.Concat(message, "\n", stackTrace));
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
