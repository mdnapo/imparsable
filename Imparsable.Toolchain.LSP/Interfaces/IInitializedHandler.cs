namespace Imparsable.Toolchain.LSP.Interfaces;

public interface IInitializedHandler : ILspMethodHandler
{
    public Task HandleAsync();
}