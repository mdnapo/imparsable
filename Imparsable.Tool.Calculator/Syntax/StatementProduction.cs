using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public abstract class StatementProduction : IProduction
{
    public static ISyntax Parse(Parser<Token>.Context context)
    {
        if (context.Match(Token.CONST))
            return ConstStatement.Parse(context);

        if (context.Match(Token.VAR))
            return VariableStatement.Parse(context);

        if (context.Match(Token.PRINT))
            return PrintStatement.Parse(context);

        return ExpressionStatement.Parse(context);
    }
}