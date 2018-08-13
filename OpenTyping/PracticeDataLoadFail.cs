using System;

namespace OpenTyping
{
    internal class PracticeDataLoadFail : Exception
    {
        public PracticeDataLoadFail() {}

        public PracticeDataLoadFail(string message)
            : base(message) {}

        public PracticeDataLoadFail(string message, Exception inner)
            : base(message, inner) {}
    }
}
