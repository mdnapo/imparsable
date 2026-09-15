using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain.Parsing;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public readonly record struct SyntaxRange(int StartLine, int StartColumn, int EndLine, int EndColumn)
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

    public bool Precedes(Position position)
    {
        // Normalize position by adding 1 to line and character
        var line = position.Line + 1;
        var character = position.Character + 1;
        return EndLine < line || EndLine == line && EndColumn < character;
    }

    public static SyntaxRange From(Lexer<Token>.Token token) => new(
        token.Line,
        token.Column,
        token.Line,
        token.Column + token.Length
    );

    public static SyntaxRange From(Lexer<Token>.Token start, Lexer<Token>.Token end) => new(
        start.Line,
        start.Column,
        end.Line,
        end.Column + end.Length
    );

    public static SyntaxRange From(Lexer<Token>.Token start, SyntaxRange end) => new(
        start.Line,
        start.Column,
        end.EndLine,
        end.EndColumn
    );
};