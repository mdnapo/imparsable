using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public static class Statement
{
    public abstract class Production : IProduction
    {
        public static ISyntax Parse(Parser<Token>.Context context)
        {
            if (context.Match(Token.CONST))
                return ConstStatement.Parse(context);

            if (context.Match(Token.VAR))
                return VarStatement.Parse(context);

            if (context.Match(Token.PRINT))
                return PrintStatement.Parse(context);

            return ExpressionStatement.Parse(context);
        }
    }
    
    public abstract class Synchronizer : ISynchronizer
    {
        public static bool Synchronize(Parser<Token>.Context context)
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
