using Imparsable.LSP.Protocol.Interfaces;

namespace Imparsable.LSP.Server.Calculator;

public class InitializedHandler : IInitializedHandler
{
    public Task HandleAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}