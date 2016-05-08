using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser
{
    public class SplitLineConfig : ILineConfig
    {
        public SplitLineConfig()
        {
        }

        public SplitLineConfig(char separator, char enclosingChar, int initialElements, Func<string> nextLineFn, Func<bool> hasNext)
        {
            _Divider = separator; 
            _EnclosingCharacter = enclosingChar;
            _InitialElements = initialElements;
            _NextLineReturn = nextLineFn;
            _hasNextLine = hasNext;
        }

        public SplitLineConfig SetSeparator(char separator)
        {
            _Divider = separator;
            return this;
        }

        public SplitLineConfig SetEnclosingCharacter(char enclosingChar = '"')
        {
            _EnclosingCharacter = enclosingChar;
            return this;
        }

        public SplitLineConfig SetInitialElementsSpace(int count)
        {
            _InitialElements = count;
            return this;
        }

        public SplitLineConfig SetHasNextLineCallback(Func<bool> fn)
        {
            _hasNextLine = fn;
            return this;
        }

        public SplitLineConfig SetTakeNextLineCallback(Func<string> fn)
        {
            _NextLineReturn = fn;
            return this;
        }

        public char Separator { get { return _Divider; } }
        public char EnclosingCharacter { get { return _EnclosingCharacter; } }
        public int InitialElements { get { return _InitialElements; } }
        public Func<string> TakeNextLine { get { return _NextLineReturn; } }
        public Func<bool> HasNextLine
        {
            get
            {
                return _hasNextLine;
            }
        }

        char _Divider;
        char _EnclosingCharacter;
        int _InitialElements;
        Func<bool> _hasNextLine;
        Func<string> _NextLineReturn;
    }
}
