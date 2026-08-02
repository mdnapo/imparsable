using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class UnaryExpression : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Operand { get; init; }
    public required Lexer<Token>.Token Op { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var op = context.Previous();
        var operand = ExpressionProduction.Parse(context);

        return new UnaryExpression
        {
            Token = op,
            Operand = operand,
            Op = op
        };
    }
}