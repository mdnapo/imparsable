using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public abstract class Expression : IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        return AssignmentExpression.Parse(context);
    }
}