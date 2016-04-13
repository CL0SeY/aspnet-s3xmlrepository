using System;

namespace S3XmlRepository
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