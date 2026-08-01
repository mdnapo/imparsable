using Imparsable.Parsing.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Tool.Calculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorConfiguration(
        this IServiceCollection services,
        Action<CalculatorParserConfiguration>? opts = null
    )
    {
        var configuration = new CalculatorParserConfiguration
        {
            Identifier =  CalculatorToken.IDENTIFIER,
            Unexpected = CalculatorToken.UNEXPECTED,
            End = CalculatorToken.END,
        };

        opts?.Invoke(configuration);

        services.AddSingleton(configuration);

        return services;
    }

    public static IServiceCollection AddCalculatorLexer(this IServiceCollection services) =>
        services
            .AddScoped<CalculatorLexer>()
            .AddIgnoreWhitespace<CalculatorToken, CalculatorParserConfiguration>()
            .AddNewLine<CalculatorToken>()
            .AddDoubleQuoteString(CalculatorToken.STRING)
            .AddIdentifier(CalculatorToken.IDENTIFIER)
            .AddNumber(CalculatorToken.NUMBER)
            .AddCharacter(CalculatorToken.PLUS, '+')
            .AddCharacter(CalculatorToken.MINUS, '-')
            .AddCharacter(CalculatorToken.STAR, '*')
            .AddCharacter(CalculatorToken.SLASH, '/')
            .AddCharacter(CalculatorToken.SEMICOLON, ';')
            .AddCharacter(CalculatorToken.EQUALS, '=')
            .AddCharacter(CalculatorToken.LEFT_PARENTHESIS, '(')
            .AddCharacter(CalculatorToken.RIGHT_PARENTHESIS, ')')
            .AddKeywords(
                CalculatorToken.CONST,
                CalculatorToken.VAR,
                CalculatorToken.PRINT
            );

}