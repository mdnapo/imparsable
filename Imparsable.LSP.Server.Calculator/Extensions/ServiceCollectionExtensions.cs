using Imparsable.LSP.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Imparsable.LSP.Server.Calculator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorLsp(this IServiceCollection services)
    {
        services.TryAddScoped<ISourceTextBuffer, SourceTextBuffer>();

        services
            .AddScoped<CalculatorLanguageServer>()
            .AddScoped<CalculatorLanguageServerConnection>();

        return services;
    }
}