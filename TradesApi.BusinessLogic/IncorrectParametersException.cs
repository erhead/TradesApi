using System;

namespace TradesApi.BusinessLogic
{
    public class IncorrectParametersException : Exception
    {
        public IncorrectParametersException() : base()
        {
        }

        public IncorrectParametersException(string message) : base(message)
        {
        }
    }
}
