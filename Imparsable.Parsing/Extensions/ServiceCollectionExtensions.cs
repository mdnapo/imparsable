using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Parsing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacter<TToken>(this IServiceCollection services, TToken type, char @char)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.CharacterRule(type, @char));

    public static IServiceCollection AddDoubleQuoteString<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.DoubleQuoteString(type));

    public static IServiceCollection AddIdentifier<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.Identifier(type));

    public static IServiceCollection AddIgnoreWhitespace<TToken, TConfiguration>(this IServiceCollection services)
        where TToken : Enum
        where TConfiguration : Parser<TToken>.Configuration =>
        services.AddScoped<Lexer<TToken>.Rule>(sp =>
            new Lexer<TToken>.Rule.IgnoreWhitespace(sp.GetRequiredService<TConfiguration>()));

    public static IServiceCollection AddNewLine<TToken>(this IServiceCollection services) where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.NewLine());

    public static IServiceCollection AddNumber<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.Number(type));

    public static IServiceCollection AddSingleQuoteString<TToken>(this IServiceCollection services, TToken type)
        where TToken : Enum =>
        services.AddScoped<Lexer<TToken>.Rule>(_ => new Lexer<TToken>.Rule.SingleQuoteString(type));

    public static IServiceCollection AddKeywords<TToken>(
        this IServiceCollection services,
        params IEnumerable<TToken> keywords
    )
        where TToken : Enum
    {
        foreach (var keyword in keywords)
            services.AddScoped(_ => new Lexer<TToken>.Keyword { Name = keyword.ToString().ToLower(), Type = keyword });

        return services;
    }
}