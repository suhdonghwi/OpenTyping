using System;

namespace OpenTyping
{
    internal class InvalidKeyLayoutDataException : Exception
    {
        public InvalidKeyLayoutDataException() {}

        public InvalidKeyLayoutDataException(string message)
            : base(message) {}

        public InvalidKeyLayoutDataException(string message, Exception inner)
            : base(message, inner) {}
    }
}