using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSVParser.Exceptions;

namespace CSVParser
{
    /// <summary>
    /// Bind property class with column name pointed at first row of CSV file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CSVHeaderNameAttribute : Attribute
    {
        string columnHeader;

        public string ColumnName { get { return columnHeader; } }

        public CSVHeaderNameAttribute(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ColumnAttributeNameNotSetException();

            columnHeader = columnName;
        }
    }
}
