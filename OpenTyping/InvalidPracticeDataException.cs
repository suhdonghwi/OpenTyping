using System;

namespace OpenTyping
{
    internal class InvalidPracticeDataException : Exception
    {
        public InvalidPracticeDataException() { }

        public InvalidPracticeDataException(string message)
            : base(message) { }

        public InvalidPracticeDataException(string message, Exception inner)
            : base(message, inner) { }
    }
}
