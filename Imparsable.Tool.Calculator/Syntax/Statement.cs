using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

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

        return ExpressionStatement.Parse(context);
    }

    public abstract class Synchronizer : ISynchronizer
    {
        public static bool Synchronize(ParserContext<Token> context)
        {
            context.Advance();

            while (!context.Ended())
            {
                if (context.Previous().Type.Equals(Token.SEMICOLON)) return true;

                switch (context.Peek().Type)
                {
                    case Token.CONST:
                    case Token.VAR:
                    case Token.PRINT:
                        return true;
                }

                context.Advance();
            }

            return true;
        }
    }
}