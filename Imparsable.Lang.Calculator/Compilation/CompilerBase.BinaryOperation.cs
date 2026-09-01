using Imparsable.Lang.Calculator.Parsing;

namespace Imparsable.Lang.Calculator.Compilation;

public partial class CompilerBase
{
    private const string ErrorMessage = "No binary compilation rule for '{0}' with '{1}' and '{2}'.";

    protected sealed record BinaryOperation(
        Token Operator,
        SystemType Left,
        SystemType Right,
        OpCode OpCode,
        SystemType? ConversionTarget = null,
        StringConversion? Conversion = null,
        EqualityType? Equality = null
    )
    {
        public override string ToString() => $"{Left} {Operator} {Right}";

        private static readonly BinaryOperation[] Signatures =
        [
            // Arithmetic
            new(Token.PLUS, SystemType.NUMBER, SystemType.NUMBER, OpCode.ADD),
            new(Token.MINUS, SystemType.NUMBER, SystemType.NUMBER, OpCode.SUB),
            new(Token.STAR, SystemType.NUMBER, SystemType.NUMBER, OpCode.MUL),
            new(Token.SLASH, SystemType.NUMBER, SystemType.NUMBER, OpCode.DIV),

            // Numeric logic
            new(Token.MODULO, SystemType.NUMBER, SystemType.NUMBER, OpCode.MOD),

            // Comparison
            new(Token.LOWER_THAN, SystemType.NUMBER, SystemType.NUMBER, OpCode.LOWER_THAN),
            new(Token.LOWER_EQUAL, SystemType.NUMBER, SystemType.NUMBER, OpCode.LOWER_EQUAL),
            new(Token.GREATER_THAN, SystemType.NUMBER, SystemType.NUMBER, OpCode.GREATER_THAN),
            new(Token.GREATER_EQUAL, SystemType.NUMBER, SystemType.NUMBER, OpCode.GREATER_EQUAL),

            // Equality
            new(Token.EQUAL_EQUAL, SystemType.NUMBER, SystemType.NUMBER, OpCode.EQUAL, Equality: EqualityType.NUMBER),
            new(Token.BANG_EQUAL, SystemType.NUMBER, SystemType.NUMBER, OpCode.NOT_EQUAL, Equality: EqualityType.NUMBER),

            new(Token.EQUAL_EQUAL, SystemType.BOOL, SystemType.BOOL, OpCode.EQUAL, Equality: EqualityType.BOOL),
            new(Token.BANG_EQUAL, SystemType.BOOL, SystemType.BOOL, OpCode.NOT_EQUAL, Equality: EqualityType.BOOL),

            new(Token.EQUAL_EQUAL, SystemType.STRING, SystemType.STRING, OpCode.EQUAL, Equality: EqualityType.STRING),
            new(Token.BANG_EQUAL, SystemType.STRING, SystemType.STRING, OpCode.NOT_EQUAL, Equality: EqualityType.STRING),

            // Logical
            new(Token.OR_OR, SystemType.BOOL, SystemType.BOOL, OpCode.OR),
            new(Token.AND_AND, SystemType.BOOL, SystemType.BOOL, OpCode.AND),

            // Concatenation
            new(Token.PLUS, SystemType.STRING, SystemType.STRING, OpCode.CONCAT),

            new(Token.PLUS, SystemType.STRING, SystemType.NUMBER, OpCode.CONCAT,
                ConversionTarget: SystemType.NUMBER, Conversion: StringConversion.NUMBER),

            new(Token.PLUS, SystemType.STRING, SystemType.BOOL, OpCode.CONCAT,
                ConversionTarget: SystemType.BOOL, Conversion: StringConversion.BOOL),
        ];

        public static BinaryOperation Resolve(Token @operator, SystemType left, SystemType right)
        {
            foreach (var signature in Signatures)
            {
                if (signature.Operator != @operator) continue;

                if ((signature.Left == left && signature.Right == right) ||
                    (signature.Left == right && signature.Right == left))
                    return signature;
            }

            throw new InvalidOperationException(string.Format(ErrorMessage, @operator, left, right));
        }
    }
}