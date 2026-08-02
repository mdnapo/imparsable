using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public abstract class LiteralExpr<T> : ISyntax
{
    public required Lexer<Token>.Token Token { get; init; }
    public required T Value { get; init; }
}