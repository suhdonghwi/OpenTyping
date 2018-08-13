using System;

namespace OpenTyping
{
    internal class KeyLayoutLoadFail : Exception
    {
        public KeyLayoutLoadFail() {}

        public KeyLayoutLoadFail(string message)
            : base(message) {}

        public KeyLayoutLoadFail(string message, Exception inner)
            : base(message, inner) {}
    }
}
