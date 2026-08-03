using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Parsing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddParserServices<TToken, TSyntax>(this IServiceCollection services)
        where TToken : Enum
        where TSyntax : ISyntax<TToken>
    {
        services
            // Add configuration
            .AddSingleton<Parser<TToken>.Configuration>()
            // Add DiagnosticsCollector
            .AddScoped<DiagnosticsCollector>()
            // Add Lexer
            .AddScoped<Lexer<TToken>.ContextProvider>()
            .AddScoped<Source>(sp => sp.GetRequiredService<Lexer<TToken>.ContextProvider>().Source)
            .AddScoped<Lexer<TToken>.Context>(sp => sp.GetRequiredService<Lexer<TToken>.ContextProvider>().GetContext())
            .AddScoped<List<Lexer<TToken>.Token>>()
            .AddScoped<Lexer<TToken>>()
            // Add Parser
            .AddScoped<List<TSyntax>>()
            .AddScoped<Parser<TToken, TSyntax>>()
            ;

        return services;
    }

    public static IServiceCollection AddSingleCharacterRule<TToken>(
        this IServiceCollection services,
        TToken type,
        char @char
    ) where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.SingleCharacterRule(type, @char));

    public static IServiceCollection AddMultiCharacterRule<TToken>(
        this IServiceCollection services,
        TToken type,
        string @string
    ) where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.MultiCharacterRule(type, @string));

    public static IServiceCollection AddDoubleQuoteStringRule<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.DoubleQuoteString(type));

    public static IServiceCollection AddIdentifierRule<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.Identifier(type));

    public static IServiceCollection AddIgnoreWhitespaceRule<TToken>(this IServiceCollection services)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(sp =>
            new Lexer<TToken>.Rule.IgnoreWhitespace(sp.GetRequiredService<Parser<TToken>.Configuration>()));

    public static IServiceCollection AddNewLineRule<TToken>(this IServiceCollection services) where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.NewLine());

    public static IServiceCollection AddNumberRule<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.Number(type));

    public static IServiceCollection AddSingleQuoteStringRule<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.SingleQuoteString(type));

    public static IServiceCollection AddKeyword<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped(_ => new Lexer<TToken>.Keyword { Name = type.ToString().ToLower(), Type = type });
}