using Imparsable.Lang.Calculator.Parsing;

namespace Imparsable.Lang.Calculator.Compilation;

public partial class CompilerBase
{
    private sealed record AssignmentOperation(Token Operator, Token? BinaryOperator)
    {
        private static readonly AssignmentOperation[] Signatures =
        [
            new(Token.EQUAL, null),
            new(Token.PLUS_EQUAL, Token.PLUS),
            new(Token.MINUS_EQUAL, Token.MINUS),
            new(Token.STAR_EQUAL, Token.STAR),
            new(Token.SLASH_EQUAL, Token.SLASH),
        ];

        public static AssignmentOperation Resolve(Token @operator)
        {
            foreach (var operation in Signatures)
                if (operation.Operator == @operator)
                    return operation;

            throw new InvalidOperationException($"No assignment compilation rule for '{@operator}'.");
        }
    }
}