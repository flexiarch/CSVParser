using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser.Exceptions
{
    /// <summary>
    /// Base Exception type for this library
    /// </summary>
    public class CSVParserException : Exception { }

    public class ColumnAttributeNameNotSetException : CSVParserException { }

    public class IncorrectElementsCountInReadedRowException : CSVParserException
    {
        private const string ERROR_ROW_NUMBER = "Number of error row";
        private const string ERROR_MSG = "Row parse error occure, invalid elements (columns) count. On row: {0}";
        private IDictionary data;
        private int errorLine;

        public IncorrectElementsCountInReadedRowException(int lineNumber)
        {
            errorLine = lineNumber;
            data = new Dictionary<string, int>
            {
                { ERROR_ROW_NUMBER, errorLine }
            };
        }

        public override string Message { get { return string.Format(ERROR_MSG, errorLine); } }
        public override IDictionary Data { get { return data; } }
    }

    public class SplitterException : CSVParserException { }
}
