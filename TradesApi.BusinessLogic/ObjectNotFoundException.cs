using System;

namespace TradesApi.BusinessLogic
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() : base()
        {
        }

        public ObjectNotFoundException(string message) : base(message)
        {
        }
    }
}
