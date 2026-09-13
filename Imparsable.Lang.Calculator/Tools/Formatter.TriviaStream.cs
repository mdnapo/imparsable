using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private sealed class TriviaStream(List<Lexer<Token>.Token> trivia) : Stream<Lexer<Token>.Token>(trivia.ToArray())
    {
        public IEnumerable<Lexer<Token>.Token?> Read(Lexer<Token>.Token token)
        {
            while (Current.Offset < token.Offset)
                yield return Advance();

            yield return null;
        }
    }
}