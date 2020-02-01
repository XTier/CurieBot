using System;

namespace Data.Core.Exceptions
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