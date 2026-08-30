using System.Reflection;
using Imparsable.Toolchain.Parsing.Attributes;
using Imparsable.Toolchain.Parsing.Exceptions;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public partial class Lexer<TToken> where TToken : Enum
{
    private static readonly IReadOnlyList<ILexerRule<TToken>> Rules = GetRules();
    public static Lexer<TToken> Default { get; } = new();

    public List<Token> Execute(Context ctx)
    {
        while (!ctx.Source.Ended())
        {
            try
            {
                if (!Rules.Any(rule => rule.Match(ctx)))
                    ctx.MarkUnexpected();
            }
            catch (SyntaxException e)
            {
                ctx.Diagnostics.Error(e.Marker, e.Message);
                ctx.Tokens.Clear();
                break;
            }
        }

        ctx.Complete();

        return ctx.Tokens;
    }

    public static IReadOnlyList<LexerRuleAttribute<TToken>> GetRules() =>
    [
        .. typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(field => field
                .GetCustomAttributes(inherit: false)
                .OfType<LexerRuleAttribute<TToken>>()
                .Select(rule => rule.SetType((TToken)field.GetValue(null)!)))
            .OrderBy(rule => rule.Priority)
    ];
}