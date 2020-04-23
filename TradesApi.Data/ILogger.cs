namespace TradesApi.Data
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogError(string message, string stackTrace);
    }
}
