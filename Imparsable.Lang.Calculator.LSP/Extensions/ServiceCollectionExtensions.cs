using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Imparsable.Lang.Calculator.LSP.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorLsp(this IServiceCollection services)
    {
        services.TryAddScoped<JsonRpcProvider>();
        services.TryAddScoped<SyntaxBuffer>();

        services
            .AddHttpContextAccessor()
            .AddScoped<CalculatorLanguageServer>()
            .AddKeyedScoped<ILspMethodHandler, InitializeHandler>(nameof(CalculatorLanguageServer))
            .AddKeyedScoped<ILspMethodHandler, InitializedHandler>(nameof(CalculatorLanguageServer))
            .AddKeyedScoped<ILspMethodHandler, TextDocumentDidOpenHandler>(nameof(CalculatorLanguageServer))
            .AddKeyedScoped<ILspMethodHandler, TextDocumentDidChangeHandler>(nameof(CalculatorLanguageServer))
            .AddKeyedScoped<ILspMethodHandler, TextDocumentDidCloseHandler>(nameof(CalculatorLanguageServer))
            .AddKeyedScoped<ILspMethodHandler, CompletionHandler>(nameof(CalculatorLanguageServer))
            ;

        return services;
    }
}