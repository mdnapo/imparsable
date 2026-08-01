using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator;

public class CalculatorParser : Parser<CalculatorToken>
{
    public interface IProduction : IProduction<ISyntax>;

    public abstract class StatementSyntax : IProduction
    {
        public static ISyntax Parse(Context context)
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

    public class ConstStatement : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required Lexer<CalculatorToken>.Token Identifier { get; init; }
        public required Lexer<CalculatorToken>.Token Assignment { get; init; }
        public required ISyntax Initializer { get; init; }
        public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

        public static ISyntax Parse(Context context)
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

    public class VariableStatement : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required Lexer<CalculatorToken>.Token Identifier { get; init; }
        public Lexer<CalculatorToken>.Token? Assignment { get; init; }
        public ISyntax? Initializer { get; set; }
        public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

        public static ISyntax Parse(Context context)
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

    public class PrintStatement : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required ISyntax Expression { get; init; }
        public required Lexer<CalculatorToken>.Token SemiColon { get; init; }

        public static ISyntax Parse(Context context)
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

    public class ExpressionStatement : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required ISyntax Expr { get; init; }
        public required Lexer<CalculatorToken>.Token SemiColon { get; set; }

        public static ISyntax Parse(Context context)
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
        public static ISyntax Parse(Context context)
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

    public class IdentifierExpression : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }

        public static ISyntax Parse(Context context) => new IdentifierExpression
        {
            Token = context.Previous()
        };
    }

    public class GroupingExpression : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required Lexer<CalculatorToken>.Token LeftParenthesis { get; init; }
        public required ISyntax Expression { get; init; }
        public required Lexer<CalculatorToken>.Token RightParenthesis { get; init; }

        public static ISyntax Parse(Context context)
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

    public class BinaryExpression : ISyntax, IProduction
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
        public required ISyntax LeftOperand { get; init; }
        public required Lexer<CalculatorToken>.Token Op { get; init; }
        public required ISyntax RightOperand { get; init; }

        public static ISyntax Parse(Context context) => Assignment(context);

        private static ISyntax Assignment(Context context) =>
            Parse(context, AssignmentOperators, AdditionSubtraction);

        private static ISyntax AdditionSubtraction(Context context) =>
            Parse(context, AdditionSubtractionOperators, MultiplicationDivision);

        private static ISyntax MultiplicationDivision(Context context) =>
            Parse(context, MultiplicationDivisionOperators, UnaryExpression.Parse);

        private static ISyntax Parse(Context context, CalculatorToken[] operators, ISyntax.Func parser)
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

    public class UnaryExpression : ISyntax, IProduction
    {
        public required Lexer<CalculatorToken>.Token Token { get; init; }
        public required ISyntax Operand { get; init; }
        public required Lexer<CalculatorToken>.Token Op { get; init; }

        public static ISyntax Parse(Context context)
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

    public abstract class LiteralExpr<T> : ISyntax
    {
        public required CalculatorLexer.Token Token { get; init; }
        public required T Value { get; init; }
    }

    public class StringLiteralExpression : LiteralExpr<string>, IProduction
    {
        public static ISyntax Parse(Context context)
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
        public static ISyntax Parse(Context context)
        {
            var previous = context.Previous();
            return new NumericLiteralExpression
            {
                Token = previous,
                Value = double.Parse(previous.Lexeme),
            };
        }
    }
}