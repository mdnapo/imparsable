using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public readonly record struct CompletionRange(int StartLine, int StartColumn, int EndLine, int EndColumn)
{
    public bool Contains(Position position)
    {
        // Normalize position by adding 1 to line and character
        var line = position.Line + 1;
        var character = position.Character + 1;

        return
            (line > StartLine || line == StartLine && character >= StartColumn) &&
            (line < EndLine || line == EndLine && character <= EndColumn);
    }
};