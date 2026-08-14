namespace Imparsable.LSP.Protocol.Interfaces;

public interface ISourceTextBuffer
{
    Task OpenAsync(string uri, string text, CancellationToken cancellationToken);
    Task UpdateAsync(string uri, string text, CancellationToken cancellationToken);
    Task GetBufferAsync(string uri, CancellationToken cancellationToken);
    Task CloseAsync(string uri, CancellationToken cancellationToken);
}