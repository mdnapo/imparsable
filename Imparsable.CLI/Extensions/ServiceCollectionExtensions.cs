using Imparsable.CLI.Commands;
using Imparsable.CLI.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.CLI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services
            .AddSingleton<Imp>()
            .AddSingleton<ISubCommandOf<Imp>, Imp.Calculator>()
            .AddSingleton<ISubCommandOf<Imp.Calculator>, Imp.Calculator.Run>()
            ;

        return services;
    }
}