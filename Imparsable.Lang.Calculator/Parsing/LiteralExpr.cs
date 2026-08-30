using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public abstract class LiteralExpr<T>
{
    public required Lexer<Token>.Token Token { get; init; }
    public required T Value { get; init; }
}