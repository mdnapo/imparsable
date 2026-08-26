namespace Imparsable.Lang.Calculator.Parsing;

public partial class TypeResolver
{
    private static class AssignmentOperation
    {
        public static SystemType Resolve(Token @operator, SystemType target, SystemType value)
        {
            if (@operator == Token.EQUAL)
                return IsAssignable(target, value) ? target : SystemType.UNKNOWN;

            Token? operation = @operator switch
            {
                Token.PLUS_EQUAL => Token.PLUS,
                Token.MINUS_EQUAL => Token.MINUS,
                Token.STAR_EQUAL => Token.STAR,
                Token.SLASH_EQUAL => Token.SLASH,
                _ => null
            };

            if (operation is null)
                return SystemType.UNKNOWN;

            var result = BinaryOperation.Resolve(operation.Value, target, value);

            if (result is SystemType.UNKNOWN)
                return SystemType.UNKNOWN;

            return IsAssignable(target, result) ? target : SystemType.UNKNOWN;
        }

        private static bool IsAssignable(SystemType target, SystemType value) => target == value;
    }
}