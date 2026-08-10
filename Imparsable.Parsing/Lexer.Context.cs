using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public class Context(Parser<TToken>.Configuration configuration, Source source, DiagnosticsProvider diagnostics)
    {
        public Parser<TToken>.Configuration Configuration { get; } = configuration;
        public Source Source { get; } = source;
        public List<Token> Tokens { get; } = [];

        public void AddToken(TToken type, string lexeme, int line = -1, int column = -1)
        {
            if (type.Equals(Configuration.Identifier))
            {
                type = Configuration.Keywords.FirstOrDefault(IsKeyword()) is { } keyword
                    ? keyword.Type
                    : type;
            }

            Tokens.Add(new Token(
                Source.File,
                type,
                lexeme,
                line == -1 ? Source.Line : line,
                column == -1 ? Source.Column : column
            ));

            return;

            Func<Keyword, bool> IsKeyword() => kw => kw.Name.Equals(lexeme, StringComparison.OrdinalIgnoreCase);
        }

        public void Halt(string message) => throw new SyntaxException(Source, message);

        public void MarkUnexpected()
        {
            int line = Source.Line, column = Source.Column;
            Source.Advance();
            var lexeme = Source.Extract();
            var token = new Token(Source.File, Configuration.Unexpected, lexeme, line, column);
            diagnostics.Error(token, $"Unexpected token '{token.Lexeme}'.");
        }

        public void Complete() => AddToken(Configuration.End, "<END>", Source.Line, Source.Column);
    }
}