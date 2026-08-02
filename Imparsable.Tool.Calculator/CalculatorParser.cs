using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator;

public interface IProduction : Parser<CalculatorToken>.IProduction<ICalculatorSyntax>;

public abstract class StatementSyntax : IProduction
{
    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        if (context.Match(CalculatorToken.CONST))
            return ConstStatement.Parse(context);

        if (context.Match(CalculatorToken.VAR))
            return VariableStatement.Parse(context);

        if (context.Match(CalculatorToken.PRINT))
            return PrintStatement.Parse(context);

        return ExpressionStatement.Parse(context);
    }
}

public class ConstStatement : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required Lexer<CalculatorToken>.Token Identifier { get; init; }
    public required Lexer<CalculatorToken>.Token Assignment { get; init; }
    public required ICalculatorSyntax Initializer { get; init; }
    public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(CalculatorToken.IDENTIFIER, "Expected an identifier.");
        var assignment = context.Consume(CalculatorToken.EQUALS, "Expected '='.");
        var initializer = ExpressionSyntax.Parse(context);
        var semiColon = context.Consume(CalculatorToken.SEMICOLON, "Expected ';'.");

        return new ConstStatement
        {
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}

public class VariableStatement : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required Lexer<CalculatorToken>.Token Identifier { get; init; }
    public Lexer<CalculatorToken>.Token? Assignment { get; init; }
    public ICalculatorSyntax? Initializer { get; set; }
    public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(CalculatorToken.IDENTIFIER, "Expected an identifier.");

        if (context.Match(CalculatorToken.SEMICOLON))
        {
            return new VariableStatement
            {
                Token = token,
                Identifier = identifier,
                SemiColon = context.Previous()
            };
        }

        var assignment = context.Consume(CalculatorToken.EQUALS, "Expected '='.");
        var initializer = ExpressionSyntax.Parse(context);
        var semiColon = context.Consume(CalculatorToken.SEMICOLON, "Expected ';'.");

        return new VariableStatement
        {
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}

public class PrintStatement : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required ICalculatorSyntax Expression { get; init; }
    public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var token = context.Previous();
        var expression = ExpressionSyntax.Parse(context);
        var semiColon = context.Consume(CalculatorToken.SEMICOLON, "Expected ';'.");

        return new PrintStatement
        {
            Token = token,
            Expression = expression,
            SemiColon = semiColon
        };
    }
}

public class ExpressionStatement : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required ICalculatorSyntax Expr { get; init; }
    public required Lexer<CalculatorToken>.Token SemiColon { get; set; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var expr = ExpressionSyntax.Parse(context);
        var semiColon = context.Consume(CalculatorToken.SEMICOLON, "Expected ';'.");
        return new ExpressionStatement
        {
            Token = expr.Token,
            Expr = expr,
            SemiColon = semiColon
        };
    }
}

public abstract class ExpressionSyntax : IProduction
{
    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        if (context.Match(CalculatorToken.IDENTIFIER))
            return IdentifierExpression.Parse(context);

        if (context.Match(CalculatorToken.LEFT_PARENTHESIS))
            return GroupingExpression.Parse(context);

        if (context.Match(CalculatorToken.STRING))
            return StringLiteralExpression.Parse(context);

        if (context.Match(CalculatorToken.NUMBER))
            return NumericLiteralExpression.Parse(context);

        return BinaryExpression.Parse(context);
    }
}

public class IdentifierExpression : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context) => new IdentifierExpression
    {
        Token = context.Previous()
    };
}

public class GroupingExpression : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required Lexer<CalculatorToken>.Token LeftParenthesis { get; init; }
    public required ICalculatorSyntax Expression { get; init; }
    public required Lexer<CalculatorToken>.Token RightParenthesis { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var leftParenthesis = context.Previous();
        var expression = ExpressionSyntax.Parse(context);
        var rightParenthesis = context.Consume(CalculatorToken.RIGHT_PARENTHESIS, "Expected ')'.");

        return new GroupingExpression
        {
            Token = leftParenthesis,
            LeftParenthesis = leftParenthesis,
            Expression = expression,
            RightParenthesis = rightParenthesis
        };
    }
}

public class BinaryExpression : ICalculatorSyntax, IProduction
{
    private static readonly CalculatorToken[] AssignmentOperators =
    [
        CalculatorToken.EQUALS
    ];

    private static readonly CalculatorToken[] AdditionSubtractionOperators =
    [
        CalculatorToken.PLUS, CalculatorToken.MINUS
    ];

    private static readonly CalculatorToken[] MultiplicationDivisionOperators =
    [
        CalculatorToken.STAR, CalculatorToken.SLASH
    ];

    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required ICalculatorSyntax LeftOperand { get; init; }
    public required Lexer<CalculatorToken>.Token Op { get; init; }
    public required ICalculatorSyntax RightOperand { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context) => Assignment(context);

    private static ICalculatorSyntax Assignment(Parser<CalculatorToken>.Context context) =>
        Parse(context, AssignmentOperators, AdditionSubtraction);

    private static ICalculatorSyntax AdditionSubtraction(Parser<CalculatorToken>.Context context) =>
        Parse(context, AdditionSubtractionOperators, MultiplicationDivision);

    private static ICalculatorSyntax MultiplicationDivision(Parser<CalculatorToken>.Context context) =>
        Parse(context, MultiplicationDivisionOperators, UnaryExpression.Parse);

    private static ICalculatorSyntax Parse(
        Parser<CalculatorToken>.Context context,
        CalculatorToken[] operators,
        ICalculatorSyntax.Func<ICalculatorSyntax> parser
    )
    {
        var expr = parser(context);

        while (context.MatchAny(operators))
        {
            var @operator = context.Previous();
            var right = parser(context);

            expr = new BinaryExpression
            {
                Token = @operator,
                LeftOperand = expr,
                Op = @operator,
                RightOperand = right
            };
        }

        return expr;
    }
}

public class UnaryExpression : ICalculatorSyntax, IProduction
{
    public required Lexer<CalculatorToken>.Token Token { get; init; }
    public required ICalculatorSyntax Operand { get; init; }
    public required Lexer<CalculatorToken>.Token Op { get; init; }

    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var op = context.Previous();
        var operand = ExpressionSyntax.Parse(context);

        return new UnaryExpression
        {
            Token = op,
            Operand = operand,
            Op = op
        };
    }
}

public abstract class LiteralExpr<T> : ICalculatorSyntax
{
    public required CalculatorLexer.Token Token { get; init; }
    public required T Value { get; init; }
}

public class StringLiteralExpression : LiteralExpr<string>, IProduction
{
    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var previous = context.Previous();
        return new StringLiteralExpression
        {
            Token = previous,
            Value = previous.Lexeme,
        };
    }
}

public class NumericLiteralExpression : LiteralExpr<double>, IProduction
{
    public static ICalculatorSyntax Parse(Parser<CalculatorToken>.Context context)
    {
        var previous = context.Previous();
        return new NumericLiteralExpression
        {
            Token = previous,
            Value = double.Parse(previous.Lexeme),
        };
    }
}