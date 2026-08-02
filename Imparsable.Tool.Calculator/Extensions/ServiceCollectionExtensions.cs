using Imparsable.Parsing.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Tool.Calculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorParser(this IServiceCollection services) =>
        services
            .AddParserServices<Token>()
            .AddIgnoreWhitespaceRule<Token>()
            .AddNewLineRule<Token>()
            .AddDoubleQuoteStringRule(Token.STRING)
            .AddSingleQuoteStringRule(Token.STRING)
            .AddIdentifierRule(Token.IDENTIFIER)
            .AddNumberRule(Token.NUMBER)
            .AddSingleCharacterRule(Token.PLUS, '+')
            .AddSingleCharacterRule(Token.MINUS, '-')
            .AddSingleCharacterRule(Token.STAR, '*')
            .AddSingleCharacterRule(Token.SLASH, '/')
            .AddSingleCharacterRule(Token.SEMICOLON, ';')
            .AddSingleCharacterRule(Token.EQUALS, '=')
            .AddSingleCharacterRule(Token.LEFT_PARENTHESIS, '(')
            .AddSingleCharacterRule(Token.RIGHT_PARENTHESIS, ')')
            .AddKeyword(Token.CONST)
            .AddKeyword(Token.VAR)
            .AddKeyword(Token.PRINT);
}