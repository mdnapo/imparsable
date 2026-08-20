using Imparsable.CLI.Commands;
using Imparsable.CLI.Extensions;
using Microsoft.Extensions.DependencyInjection;

try
{
    var cancellation = new CancellationTokenSource();
    Console.CancelKeyPress += (s, e) =>
    {
        cancellation.Cancel();
        e.Cancel = true;
    };

    var services = new ServiceCollection()
        .AddCommands()
        .BuildServiceProvider();

    await services
        .GetRequiredService<Imp>()
        .ExecuteAsync(args, cancellation.Token);
}
catch (Exception ex)
{
    await Console.Error.WriteAsync(ex.Message);
    Environment.ExitCode = 1;
}