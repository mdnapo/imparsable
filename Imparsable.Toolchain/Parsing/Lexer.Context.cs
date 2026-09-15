using Imparsable.Toolchain.Parsing.Exceptions;
using Imparsable.Toolchain.Parsing.Extensions;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public partial class Lexer<TToken>
{
    public class Context(ParserConfiguration<TToken> configuration, DiagnosticsProvider diagnostics, Source source)
    {
        public ParserConfiguration<TToken> Configuration { get; } = configuration;
        public Source Source { get; } = source;
        public DiagnosticsProvider Diagnostics { get; } = diagnostics;
        public List<Token> Tokens { get; } = [];
        public List<Token> Trivia { get; } = [];
        public Dictionary<Token, Range> TriviaIndex { get; } = [];
        private int TriviaPointer { get; set; }

        public void AddToken(TToken type, int offset, int length, int line, int column)
        {
            if (type.IsIdentifier<TToken>())
            {
                var text = Source.GetTextSpan(offset, length);
                type = Configuration.IsKeyword(text) is { } keyword
                    ? keyword.Type
                    : type;
            }

            var token = new Token(type, offset, length, line, column);

            if (Configuration.IsTrivia(type))
            {
                Trivia.Add(token);
            }
            else
            {
                Tokens.Add(token);
                IndexTrivia(token);
            }
        }

        public void Halt(ISourceMarker marker, string message) => throw new SyntaxException(marker, message);

        public void MarkUnexpected()
        {
            int line = Source.Line, column = Source.Column;

            Source.Advance();

            var range = Source.Extract();
            var token = new Token(Configuration.Unexpected, range.Offset, range.Length, line, column);
            var text = Source.GetTextSpan(range.Offset, range.Length);

            Diagnostics.Error(token, $"Unexpected token '{text}'.");
        }

        public void Complete()
        {
            var range = Source.Extract();
            AddToken(Configuration.End, range.Offset, range.Length, Source.Line, Source.Column);
        }

        private void IndexTrivia(Token token)
        {
            if (TriviaPointer == Trivia.Count) return;

            var start = TriviaPointer;
            var end = Trivia.Count - TriviaPointer;
            TriviaIndex[token] = new Range(start, start + end);
            TriviaPointer = Trivia.Count;
        }
    }
}