using System.CommandLine;
using Imparsable.CLI.Interfaces;

namespace Imparsable.CLI.Extensions;

internal static class CommandExtensions
{
    private static readonly InvocationConfiguration InvocationConfiguration = new()
    {
        EnableDefaultExceptionHandler = false,
    };

    public static void RegisterSubCommands<TCommand>(
        this TCommand target,
        IEnumerable<ISubCommandOf<TCommand>> commands
    ) where TCommand : Command
    {
        foreach (var command in commands)
            target.Subcommands.Add(command as Command ?? throw new InvalidCastException());
    }

    public static async Task<int> ExecuteAsync(
        this RootCommand command,
        string[] args,
        CancellationToken cancellationToken
    ) =>
        await command.Parse(args).InvokeAsync(
            configuration: InvocationConfiguration,
            cancellationToken: cancellationToken);
}