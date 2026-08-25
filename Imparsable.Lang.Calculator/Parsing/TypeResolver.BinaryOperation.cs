namespace Imparsable.Lang.Calculator.Parsing;

public partial class TypeResolver
{
    private sealed record BinaryOperation(Token Operator, SystemType Left, SystemType Right, SystemType Result)
    {
        private static readonly BinaryOperation[] Signatures =
        [
            // Arithmetic
            new(Token.PLUS, SystemType.NUMBER, SystemType.NUMBER, SystemType.NUMBER),
            new(Token.MINUS, SystemType.NUMBER, SystemType.NUMBER, SystemType.NUMBER),
            new(Token.STAR, SystemType.NUMBER, SystemType.NUMBER, SystemType.NUMBER),
            new(Token.SLASH, SystemType.NUMBER, SystemType.NUMBER, SystemType.NUMBER),

            // Numeric comparison
            new(Token.LOWER_THAN, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),
            new(Token.LOWER_EQUAL, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),
            new(Token.GREATER_THAN, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),
            new(Token.GREATER_EQUAL, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),

            // Numeric equality
            new(Token.EQUAL_EQUAL, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),
            new(Token.BANG_EQUAL, SystemType.NUMBER, SystemType.NUMBER, SystemType.BOOL),

            // Boolean equality
            new(Token.EQUAL_EQUAL, SystemType.BOOL, SystemType.BOOL, SystemType.BOOL),
            new(Token.BANG_EQUAL, SystemType.BOOL, SystemType.BOOL, SystemType.BOOL),

            // String equality
            new(Token.EQUAL_EQUAL, SystemType.STRING, SystemType.STRING, SystemType.BOOL),
            new(Token.BANG_EQUAL, SystemType.STRING, SystemType.STRING, SystemType.BOOL),

            // String concatenation
            new(Token.DOT, SystemType.STRING, SystemType.STRING, SystemType.STRING),
            new(Token.DOT, SystemType.STRING, SystemType.BOOL, SystemType.STRING),
            new(Token.DOT, SystemType.STRING, SystemType.NUMBER, SystemType.STRING),
        ];

        public static SystemType Resolve(Token @operator, SystemType left, SystemType right)
        {
            foreach (var operation in Signatures)
            {
                if (operation.Operator != @operator) continue;

                if (operation.Left == left && operation.Right == right ||
                    operation.Left == right && operation.Right == left)
                    return operation.Result;
            }

            return SystemType.UNKNOWN;
        }
    }
}