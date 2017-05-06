using System;

namespace DataHandling.Core.Exceptions
{
    // TODO rename 
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}