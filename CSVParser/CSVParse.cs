using CSVParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSVParser
{
    /// <summary>
    /// CSV Parser
    /// Save export to entity class with properties marked with CSVColumnNameAttribute.
    /// </summary>
    /// <typeparam name="TResult">DTO class, properties mustbe marked with CSVCOlumnNameAttribute</typeparam>
    public class CSVParser<TResult> : IDisposable where TResult : class
    {
        public bool HasHeaderOnFirstRow { get; set; }

        private char[] delimiters = new char[] { ',' };
        private string[] headerLine;
        private int HeaderColumnsCount { get { return headerLine?.Length ?? 0; } }
        private IDictionary<string, int> binder;
        private ILineConfig spliterCfg;

        Type typeTResult;
        StreamReader sr;

        public CSVParser(StreamReader readingStream)
        {
            typeTResult = typeof(TResult);
            sr = readingStream;

            this.spliterCfg = new SplitLineConfig()
                    .SetEnclosingCharacter('"')
                    .SetSeparator(',')
                    .SetInitialElementsSpace(1)
                    .SetHasNextLineCallback(ICanRead)
                    .SetTakeNextLineCallback(ReadLine)
            ;
        }

        public CSVParser(StreamReader readingStream, bool hasHeaderRow)
            :this(readingStream)
        {
            HasHeaderOnFirstRow = hasHeaderRow;
        }

        public CSVParser<TResult> SetHeaderRow(params string[] headers)
        {
            headerLine = headers;
            BuildBindings();
            UpdateElementsCount();
            return this;
        }

        public CSVParser<TResult> SetHeaderRow(string headerToSplit)
        {
            return SetHeaderRow(SplitLine(headerToSplit));
        }

        /// <summary>
        /// Parse stream line by line to the end.
        /// </summary>
        /// <param name="LineParsed">delegate with parameter of newly created TResult type object</param>
        public IEnumerable<TResult>Parse()
        {
            string line;
            string[] splitedLine;
            int currentLine = 0;

            if (HasHeaderOnFirstRow)
            {
                ReadHeaderRow();
                BuildBindings();
                currentLine = 1;
            }

            while ((line = ReadLine()) != null)
            {
                currentLine++;
                splitedLine = SplitLine(line);

                if (!IsCorrectElementCount(splitedLine))
                    throw new IncorrectElementsCountInReadedRowException(currentLine);

                yield return ConstructEntity(splitedLine);
            }
        }

        /// <summary>
        /// Read line and tract it as header line; Usualy first line of CSV flat file
        /// </summary>
        private void ReadHeaderRow()
        {
            headerLine = SplitLine(ReadLine());
            UpdateElementsCount();
        }

        /// <summary>
        /// Gets line from stream
        /// </summary>
        /// <returns></returns>
        private string ReadLine()
        {
            return sr.ReadLine();
        }

        private bool ICanRead()
        {
            return !sr.EndOfStream;
        }

        /// <summary>
        /// Splits string by delemiter
        /// </summary>
        private string[] SplitLine(string n)
        {
            var splitter = new SplitLine(n, spliterCfg);
            if (!splitter.Process())
                throw new SplitterException();

            return splitter.Result;
            //return n.Split(delimiters);
        }

        private void UpdateElementsCount()
        {
            ((SplitLineConfig)spliterCfg).SetInitialElementsSpace(headerLine.Length);
        }

        /// <summary>
        /// All rows must have the same count of elements as header row
        /// </summary>
        private bool IsCorrectElementCount(string[] splitedLine)
        {
            return splitedLine.Length == HeaderColumnsCount;
        }

        /// <summary>
        /// Create new instance of TResult and fill selected properties
        /// </summary>
        private TResult ConstructEntity(string[] splited)
        {
            var newEntity = Activator.CreateInstance<TResult>();

            foreach (var item in binder)
            {
                PropertyInfo prop = typeTResult.GetProperty(item.Key);
                prop.SetValue(newEntity, splited[item.Value]);
            }

            return newEntity;
        }

        /// <summary>
        /// Binds TResult class property to CSV Column header name;
        /// Select properties from TResult class marked with CSVColumnNameAttribute.
        /// Creates binding dictionary with TResult property name (key) and index of CSV header text.
        /// </summary>
        private void BuildBindings()
        {
            binder = new Dictionary<string, int>();

            // gets all properties market of CSVHeaderNameAttribute
            var props = typeof(TResult).GetProperties()
                .Where(
                    prop =>
                    prop.GetCustomAttributes(typeof(CSVHeaderNameAttribute), true).Length == 1);

            int indexOfColumn;
            foreach (var prop in props)
            {
                var csvHeaderName = (
                    (CSVHeaderNameAttribute)Attribute.GetCustomAttribute(prop, typeof(CSVHeaderNameAttribute))
                    ).ColumnName;

                if ((indexOfColumn = Array.IndexOf<string>(headerLine, csvHeaderName)) > -1) 
                    binder.Add(prop.Name, indexOfColumn);
            }
        }

        /// <summary>
        /// release StreamReader
        /// </summary>
        public void Dispose()
        {
            if (sr != null)
                sr.Dispose();
        }
    }
}
