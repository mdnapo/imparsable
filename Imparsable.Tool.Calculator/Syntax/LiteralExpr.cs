using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public abstract class LiteralExpr<T>
{
    public required Lexer<Token>.Token Token { get; init; }
    public required T Value { get; init; }
}