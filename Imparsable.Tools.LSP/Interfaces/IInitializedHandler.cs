namespace Imparsable.Tools.LSP.Interfaces;

public interface IInitializedHandler : ILspMethodHandler
{
    public Task HandleAsync(CancellationToken cancellationToken);
}