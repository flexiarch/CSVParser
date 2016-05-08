using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using CSVParser;
using System.Collections.Generic;
using System.Linq;
using MSTestExtensions;

namespace CSVParser.Tests
{
    [TestClass]
    public class CSVTests : BaseTest
    {
        [TestMethod]
        public void csv_parse_library_works()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\"V1\",\"V2");
            sb.AppendLine("vv22vv\"");
            sb.AppendLine("C1,C2");

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

            StreamReader sr = new StreamReader(ms);

            List<MYTest> resultList = new List<MYTest>();
            using (var parser = new CSVParser<MYTest>(sr))
            {
                parser.SetHeaderRow("Column1", "Column2");
                resultList.AddRange(parser.Parse());
            }

            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("V1", resultList[0].Aaa);
            Assert.AreEqual("V2\nvv22vv", resultList[0].Bbb);
            Assert.AreEqual("C1", resultList[1].Aaa);
            Assert.AreEqual("C2", resultList[1].Bbb);
        }

        [TestMethod]
        public void csv_stream_has_a_header_row()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Column1,Column2"); // Header columns name on first line
            sb.AppendLine("V1,V2"); // row 1
            sb.AppendLine("C1,C2"); // row 2

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

            StreamReader sr = new StreamReader(ms);

            List<MYTest> resultList = new List<MYTest>();

            using (var parser = new CSVParser<MYTest>(sr, true))
                resultList.AddRange(parser.Parse());

            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("V1", resultList[0].Aaa);
            Assert.AreEqual("V2", resultList[0].Bbb);
            Assert.AreEqual("C1", resultList[1].Aaa);
            Assert.AreEqual("C2", resultList[1].Bbb);
        }

    [TestMethod]
    //[ExpectedException(typeof(CSVParser.Exceptions.IncorrectElementsCountInReadedRowException))]
    public void csv_stream_has_error_row_row()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Column1,Column2");   // row 1
        sb.AppendLine("V1,V2");             // row 2
        sb.AppendLine("C1,C2,Err");         // row 3

        var ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

        StreamReader sr = new StreamReader(ms);

        List<MYTest> resultList = new List<MYTest>();

            Assert.Throws<CSVParser.Exceptions.IncorrectElementsCountInReadedRowException>(() =>
            {
                using (var parser = new CSVParser<MYTest>(sr, true))
                    resultList.AddRange(parser.Parse());
            },
                "Row parse error occure, invalid elements (columns) count. On row: 3"
            );
    }
}


class MYTest
    {
        [CSVHeaderName("Column1")]
        public string Aaa { get; set; }

        [CSVHeaderName("Column2")]
        public string Bbb { get; set; }
    }
}
