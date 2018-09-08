using System;

namespace OpenTyping
{
    internal class InvalidKeyDataException : Exception
    {
        public InvalidKeyDataException() {}

        public InvalidKeyDataException(string message)
            : base(message) {}

        public InvalidKeyDataException(string message, Exception inner)
            : base(message, inner) {}
    }
}