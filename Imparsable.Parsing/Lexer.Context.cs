namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public class Context(Parser<TToken>.Configuration configuration, string file, Source source, List<Token> tokens)
    {
        public string File { get; } = file;
        public Source Source { get; } = source;
        public List<Token> Tokens { get; } = tokens;

        public void AddToken(TToken type, string lexeme, int line = -1, int column = -1)
        {
            Tokens.Add(new Token(
                File,
                type,
                lexeme,
                line == -1 ? Source.Line : line,
                column == -1 ? Source.Column : column
            ));
        }

        public void MarkUnexpected()
        {
            int line = Source.Line, column = Source.Column;
            Source.Advance();
            var lexeme = Source.Extract();
            AddToken(configuration.Unexpected, lexeme, line, column);
        }
    }
}