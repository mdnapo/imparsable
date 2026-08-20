using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class ConstStatement : ISyntax, ISymbol, IProduction
{
    public required Source Source { get; init; }
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public required Lexer<Token>.Token Assignment { get; init; }
    public required ISyntax Initializer { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }
    public string Symbol => Source.GetText(Identifier.Offset,  Identifier.Length);

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Syntax.Token.IDENTIFIER, "Expected an identifier.");
        var assignment = context.Consume(Syntax.Token.EQUAL, "Expected '='.");
        var initializer = Expression.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");

        return new ConstStatement
        {
            Source =  context.Source,
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}