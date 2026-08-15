namespace Imparsable.LSP.Protocol.Interfaces;

public interface IInitializedHandler : ILspMethodHandler
{
    public Task HandleAsync(CancellationToken cancellationToken);
}