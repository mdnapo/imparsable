using Imparsable.LSP.Protocol;
using Imparsable.LSP.Protocol.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Imparsable.LSP.Server.Calculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorLsp(this IServiceCollection services)
    {
        services.TryAddScoped<JsonRpcProvider>();
        services.TryAddScoped<ISourceTextBuffer, SourceTextBuffer>();

        services
            .AddHttpContextAccessor()
            .AddScoped<CalculatorLanguageServer>()
            .AddKeyedScoped<ILspMethodHandler, CalculatorCompletionHandler>(nameof(CalculatorLanguageServer))
            ;

        return services;
    }
}