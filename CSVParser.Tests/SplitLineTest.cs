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
    public class SplitLineTest
    {
        [TestMethod]
        public void normal_spliting_by_divider()
        {
            var to = new SplitLine("aaa,bbb,ccc");
            
            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
            Assert.AreEqual<string>("ccc", to.Result[2]);
        }
        [TestMethod]
        public void enclosed_values()
        {
            var to = new SplitLine("\"aaa\",\"bbb\",\"ccc\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
            Assert.AreEqual<string>("ccc", to.Result[2]);
        }
        [TestMethod]
        public void empty_values()
        {
            var to = new SplitLine(",,\"ccc\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>("", to.Result[0]);
            Assert.AreEqual<string>("", to.Result[1]);
            Assert.AreEqual<string>("ccc", to.Result[2]);
        }
        [TestMethod]
        public void empty_values_enclosed()
        {
            var to = new SplitLine("\"\",\"\",\"ccc\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>("", to.Result[0]);
            Assert.AreEqual<string>("", to.Result[1]);
            Assert.AreEqual<string>("ccc", to.Result[2]);
        }
        [TestMethod]
        public void can_enclose_divider_character()
        {
            var to = new SplitLine("\" ,\",\"bbb\",\",\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>(" ,", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
            Assert.AreEqual<string>(",", to.Result[2]);
        }

        [TestMethod]
        public void before_enclosing_can_be_white_character()
        {
            var to = new SplitLine("    \" ,\",\"bbb\",\",\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(3, to.Result.Length);
            Assert.AreEqual<string>(" ,", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
            Assert.AreEqual<string>(",", to.Result[2]);
        }

        [TestMethod]
        public void after_enclosing_can_be_white_characters()
        {
            var testString = string.Concat("aaa,\"bbb\"", new String(' ', 4));
            var to = new SplitLine(testString);

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
        }

        [TestMethod]
        public void white_chars_at_begining_before_quotation_char()
        {
            var testString = string.Concat(new String(' ', 4), "\"aaa\",bbb");
            var to = new SplitLine(testString);

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
        }

        [TestMethod]
        public void white_chars_after_ending_quotation_char()
        {
            var testString = string.Concat("\"aaa\"", new String(' ', 4), ",bbb");
            var to = new SplitLine(testString);

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
            Assert.AreEqual<string>("bbb", to.Result[1]);
        }

        [TestMethod]
        public void duble_enclosing_character_as_escape_char()
        {
            var to = new SplitLine("1,\"Quoted \"\"Value\"\"\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("1", to.Result[0]);
            Assert.AreEqual<string>("Quoted \"Value\"", to.Result[1]);
        }

        [TestMethod]
        public void single_value()
        {
            var to = new SplitLine("aaa");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(1, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
        }

        [TestMethod]
        public void single_enclosed_value()
        {
            var to = new SplitLine("\"aaa\"");

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(1, to.Result.Length);
            Assert.AreEqual<string>("aaa", to.Result[0]);
        }
        /// <summary>
        /// CRLF chars must be sourrunded with double quotes.
        /// 
        /// </summary>
        [TestMethod]
        public void need_of_next_line()
        {
            string[] lines = {
                "1,\"Quoted,",
                "breaked string\"" };

            var cfg = new LineCfg(',', '"', 2, () => { return lines[1]; }, () => { return true; });

            var to = new SplitLine(lines[0], cfg);

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("1", to.Result[0]);
            Assert.AreEqual<string>("Quoted,\nbreaked string", to.Result[1]);
        }

        [TestMethod]
        public void need_of_next_line_2()
        {
            string[] lines = {
                "1,\"Quoted,",          // \n
                "",                     // \n
                "",                     // \n
                "",                     // \n
                "breaked string\"" };   // \n


            int index = 0;

            var cfg = new LineCfg(',', '"', 2, () => { return lines[++index]; }, () => { return index<lines.Length; });
            var to = new SplitLine(lines[index], cfg);

            Assert.AreEqual<bool>(true, to.Process());
            Assert.AreEqual<int>(2, to.Result.Length);
            Assert.AreEqual<string>("1", to.Result[0]);
            Assert.AreEqual<string>("Quoted,\n\n\n\nbreaked string", to.Result[1]);
        }



        public class LineCfg : ILineConfig
        {
            public LineCfg(char Divider, char EnclosingChar, int InitialElements, Func<string> NextLineFn, Func<bool> hasNext)
            {
                _Divider = Divider;
                _EnclosingCharacter = EnclosingChar;
                _InitialElements = InitialElements;
                _NextLineReturn = NextLineFn;
                _hasNextLine = hasNext;
            }

            public char Separator { get { return _Divider; } }
            public char EnclosingCharacter { get { return _EnclosingCharacter; } }
            public int InitialElements { get { return _InitialElements;} }
            public Func<string> TakeNextLine { get { return _NextLineReturn;} }

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
}
