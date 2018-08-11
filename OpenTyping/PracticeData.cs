using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTyping
{
    class PracticeData
    {
        public PracticeData(IList<string> textData, string character)
        {
            TextData = textData;
            Character = character;
        }

        public IList<string> TextData { get; }
        public string Character { get; }
    }
}
