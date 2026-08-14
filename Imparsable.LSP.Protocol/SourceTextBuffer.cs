using Imparsable.LSP.Protocol.Interfaces;

namespace Imparsable.LSP.Protocol;

public class SourceTextBuffer : ISourceTextBuffer
{
    private readonly Dictionary<string, string> _sources = [];

    public Task OpenAsync(string uri, string text, CancellationToken cancellationToken)
    {
        _sources[uri] = text;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(string uri, string text, CancellationToken cancellationToken)
    {
        _sources[uri] = text;
        return Task.CompletedTask;
    }

    public Task GetBufferAsync(string uri, CancellationToken cancellationToken) => Task.FromResult(_sources[uri]);

    public Task CloseAsync(string uri, CancellationToken cancellationToken)
    {
        _sources.Remove(uri);
        return Task.CompletedTask;
    }
}