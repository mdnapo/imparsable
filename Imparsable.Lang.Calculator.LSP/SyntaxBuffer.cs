using System.Collections.Concurrent;
using Imparsable.Lang.Calculator.Parsing;

namespace Imparsable.Lang.Calculator.LSP;

public class SyntaxBuffer
{
    private readonly ConcurrentDictionary<string, SyntaxTree> _sources = [];

    public Task OpenAsync(string uri, string text, CancellationToken cancellationToken)
    {
        _sources[uri] = SyntaxTree.Parse(text);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(string uri, string text, CancellationToken cancellationToken)
    {
        _sources[uri] = SyntaxTree.Parse(text);
        return Task.CompletedTask;
    }

    public Task<SyntaxTree> GetBufferAsync(string uri, CancellationToken cancellationToken) =>
        Task.FromResult(_sources[uri]);

    public Task CloseAsync(string uri, CancellationToken cancellationToken)
    {
        _sources.Remove(uri, out _);
        return Task.CompletedTask;
    }
}