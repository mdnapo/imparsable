using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public abstract class Statement : IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        if (context.Match(Token.CONST))
            return ConstStatement.Parse(context);

        if (context.Match(Token.VAR))
            return VarStatement.Parse(context);

        if (context.Match(Token.PRINT))
            return PrintStatement.Parse(context);

        if (context.Match(Token.LEFT_BRACE))
            return BlockStatement.Parse(context);

        if (context.Match(Token.FOR))
            return ForStatement.Parse(context);

        if (context.Match(Token.WHILE))
            return WhileStatement.Parse(context);

        if (context.Match(Token.BREAK))
            return BreakStatement.Parse(context);

        if (context.Match(Token.CONTINUE))
            return ContinueStatement.Parse(context);

        if (context.Match(Token.IF))
            return IfStatement.Parse(context);

        return ExpressionStatement.Parse(context);
    }
}