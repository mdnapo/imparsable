using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class ErrorNode : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }

    public static ISyntax Parse(ParserContext<Token> context) =>
        new ErrorNode { Token = context.Current };
}