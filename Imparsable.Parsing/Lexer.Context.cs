using Imparsable.Parsing.Exceptions;
using Imparsable.Parsing.Extensions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public class Context(ParserConfiguration<TToken> configuration, Source source, DiagnosticsProvider diagnostics)
    {
        public ParserConfiguration<TToken> Configuration { get; } = configuration;
        public Source Source { get; } = source;
        public DiagnosticsProvider Diagnostics { get; } = diagnostics;
        public List<Token> Tokens { get; } = [];

        public void AddToken(TToken type, string lexeme, int line = -1, int column = -1)
        {
            if (type.IsIdentifier<TToken>())
            {
                type = ParserConfiguration<TToken>.IsKeyword(lexeme) is { } keyword
                    ? keyword.Type
                    : type;
            }

            Tokens.Add(new Token(
                type,
                lexeme,
                line == -1 ? Source.Line : line,
                column == -1 ? Source.Column : column
            ));
        }

        public void Halt(string message) => throw new SyntaxException(Source, message);

        public void MarkUnexpected()
        {
            int line = Source.Line, column = Source.Column;
            Source.Advance();
            var lexeme = Source.Extract();
            var token = new Token(Configuration.Unexpected, lexeme, line, column);
            Diagnostics.Error(token, $"Unexpected token '{token.Lexeme}'.");
        }

        public void Complete() => AddToken(Configuration.End, "<END>", Source.Line, Source.Column);
    }
}