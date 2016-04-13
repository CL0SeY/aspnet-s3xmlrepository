using System;

namespace aspnet_s3_xmlrepository
{
    internal class TooManyObjectsException : Exception
    {
        public TooManyObjectsException()
        {
        }

        public TooManyObjectsException(string message) : base(message)
        {
        }

        public TooManyObjectsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}