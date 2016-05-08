using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser
{
    internal class SplitLineDefaultConfig : ILineConfig
    {
        public char Separator
        {
            get
            {
                return ',';
            }
        }

        public char EnclosingCharacter
        {
            get
            {
                return '"';
            }
        }

        public Func<bool> HasNextLine { get { return () => { return false; }; } }

        public int InitialElements { get{return 1;}  }

        public Func<string> TakeNextLine { get { return null; }  }


    }
}
