using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CSVParser.Tests")]
[assembly: InternalsVisibleTo("RunTimeTests")]
namespace CSVParser
{
    internal class SplitLine
    {
        ILineConfig cfg;
        string line;
        int lineLength;

        string timedEnclosing, singleEnclosing;

        List<string> results;
        public string[] Result { get { return results.ToArray(); } }
        public bool Process()
        {
            int start, stop, div;
            bool doubleQouted = false;

            for (int i = 0; i < lineLength; i++, doubleQouted=false)
            {
                if (line[i] == cfg.EnclosingCharacter)
                {
                    start = i+1;
                    if (DoubledEnclosings(i))
                    {
                        Add();
                        i = start+1;
                    }
                    else if (ScanFor(cfg.EnclosingCharacter, start, out stop, out doubleQouted))
                    {
                        if ((stop - start) == 0) // ""
                        {
                            // add empty string to collection
                            Add();
                            i = stop;
                        }
                        else if (ScanFor(cfg.Separator, stop + 1,  out div))
                        {
                            // Sub(start, stop);
                            Add(start, stop, doubleQouted);
                            i = div;
                        }
                        else if (lineLength - 1 == stop)
                        {
                            // Sub(start, stop);
                            Add(start, stop, doubleQouted);
                            // its a end position string
                            i = stop;
                        }
                        else if (line.Substring(stop+1).All(p => char.IsWhiteSpace(p)))
                        {
                            Add(start, stop, doubleQouted);
                            i = stop;
                        }
                        else
                        {
                            // Set error on character no: stop, no divider found
                            return false;
                        }
                    }
                    else
                    {
                        if (cfg.TakeNextLine == null)
                        {
                            return false;
                        }
                        else
                        {
                            if (cfg.HasNextLine())
                            {
                                line += "\n" + cfg.TakeNextLine();
                                lineLength = line.Length;
                                i = start - 2;
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else if (i == lineLength && line[i] == cfg.Separator)
                {
                    // if , is last char on right then
                    // add empty string
                    Add();
                }
                else if (char.IsWhiteSpace(line, i))
                {
                    // goto next iteration
                    continue;
                }
                else if (line[i] != cfg.Separator)
                {
                    start = i;
                    if (ScanFor(cfg.Separator, i + 1, out stop, out doubleQouted))
                    {
                        Add(start, stop, doubleQouted);
                        i = stop;
                    }
                    else
                    {
                        Add(start, lineLength, doubleQouted);
                        i = lineLength;
                    }
                }
                else if (line[i] == cfg.Separator)
                {
                    Add();
                }
            }

            return true;
        }
        // " (n)  ,
        private bool ScanFor(char chr, int start, out int result)
        {
            for (int i = start; i < lineLength; i++)
            {
                if (line[i] == chr)
                {
                    result = i;
                    return true;
                }
                else if (!char.IsWhiteSpace(line, i))
                {
                    break;
                }
            }
            result = -1;
            return false;
        }

        private bool ScanFor(char chr, int start, out int result, out bool hasDoubleQuotes)
        {
            bool _hasDoubleQuotes = false;
            for (int i = start; i < lineLength; i++)
            {
                if (DoubledEnclosings(i))
                {
                    _hasDoubleQuotes = true;
                    i ++;
                    continue;
                }
                else if (line[i] == chr)
                {
                    result = i;
                    hasDoubleQuotes = _hasDoubleQuotes;
                    return true;
                }
            }
            result = -1;
            hasDoubleQuotes = _hasDoubleQuotes;
            return false;
        }

        private bool DoubledEnclosings(int start)
        {
            return line[start] == cfg.EnclosingCharacter &&
                    start < (lineLength - 1) &&
                    line[start + 1] == cfg.EnclosingCharacter;
        }

        private void Add()
        {
            results.Add(string.Empty);
        }

        private void Add (int start, int stop, bool dq)
        {
            var sub = line.Substring(start, stop - start);
            if (dq)
                results.Add(sub.Replace(timedEnclosing, singleEnclosing));
            else
                results.Add(sub);
        }

        public SplitLine(string s)
        {
            Setup(s, new SplitLineDefaultConfig());
        }

        public SplitLine(string s, ILineConfig cfg)
        {
            Setup(s, cfg);
        }

        private void Setup(string s, ILineConfig cfg)
        {
            this.cfg = cfg;
            line = s;
            lineLength = s.Length;
            results = new List<string>(cfg.InitialElements);
            timedEnclosing = new string(cfg.EnclosingCharacter, 2);
            singleEnclosing = new string(cfg.EnclosingCharacter, 1);
        }
    }
}
