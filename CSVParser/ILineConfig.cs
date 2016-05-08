using System;

namespace CSVParser
{
    public interface ILineConfig
    {
        char Separator { get; }
        char EnclosingCharacter { get; }
        int InitialElements { get; }
        Func<string> TakeNextLine { get;}
        Func<bool> HasNextLine { get; }
    }
}
