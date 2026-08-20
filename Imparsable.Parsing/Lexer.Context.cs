using Imparsable.Parsing.Exceptions;
using Imparsable.Parsing.Extensions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public class Context(ParserConfiguration<TToken> configuration, DiagnosticsProvider diagnostics, Source source)
    {
        public ParserConfiguration<TToken> Configuration { get; } = configuration;
        public Source Source { get; } = source;
        public DiagnosticsProvider Diagnostics { get; } = diagnostics;
        public List<Token> Tokens { get; } = [];

        public void AddToken(TToken type, int offset, int length, int line = -1, int column = -1)
        {
            var text = Source.GetText(offset, length);

            if (type.IsIdentifier<TToken>())
            {
                type = ParserConfiguration<TToken>.IsKeyword(text) is { } keyword
                    ? keyword.Type
                    : type;
            }

            line = line == -1 ? Source.Line : line;
            column = column == -1 ? Source.Column : column;

            Tokens.Add(new Token(type, offset, length, line, column));
        }

        public void Halt(string message) => throw new SyntaxException(Source, message);

        public void MarkUnexpected()
        {
            int line = Source.Line, column = Source.Column;

            Source.Advance();

            var range = Source.Extract();
            var token = new Token(Configuration.Unexpected, range.Offset, range.Length, line, column);
            var text = Source.GetText(range.Offset, range.Length);

            Diagnostics.Error(token, $"Unexpected token '{text}'.");
        }

        public void Complete()
        {
            var range = Source.Extract();
            AddToken(Configuration.End, range.Offset, range.Length, Source.Line, Source.Column);
        }
    }
}