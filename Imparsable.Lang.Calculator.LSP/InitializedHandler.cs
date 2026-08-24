using Imparsable.Tools.LSP.Interfaces;

namespace Imparsable.Lang.Calculator.LSP;

public class InitializedHandler : IInitializedHandler
{
    public Task HandleAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}