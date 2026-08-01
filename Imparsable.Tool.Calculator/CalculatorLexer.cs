using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator;

public class CalculatorLexer(
    CalculatorParserConfiguration configuration,
    IEnumerable<Lexer<CalculatorToken>.Rule> rules,
    IEnumerable<Lexer<CalculatorToken>.Keyword> keywords
) : Lexer<CalculatorToken>(configuration, rules, keywords)
{
    protected override CalculatorToken End => CalculatorToken.END;
}