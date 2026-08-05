using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public partial class Parser<TToken, TSyntax>(
    Lexer<TToken> lexer,
    List<TSyntax> syntax,
    DiagnosticsProvider diagnostics
)
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
{
    public List<TSyntax> Execute<TProduction>() where TProduction : IProduction
    {
        var context = lexer.Execute();

        while (!context.Ended())
        {
            try
            {
                syntax.Add(TProduction.Parse(context));
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                break;
            }
        }

        return syntax;
    }

    public List<TSyntax> Execute<TProduction, TSynchronizer>()
        where TProduction : IProduction
        where TSynchronizer : ISynchronizer
    {
        var context = lexer.Execute();

        while (!context.Ended())
        {
            try
            {
                syntax.Add(TProduction.Parse(context));
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                if (!TSynchronizer.Synchronize(context))
                    break;
            }
        }

        return syntax;
    }
}