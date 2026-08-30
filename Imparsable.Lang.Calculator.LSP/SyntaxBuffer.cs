using System.Collections.Concurrent;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;

namespace Imparsable.Lang.Calculator.LSP;

public class SyntaxBuffer
{
    private readonly ConcurrentDictionary<string, SyntaxTree> _sources = [];

    public void OpenAsync(string uri, string text, DiagnosticsProvider diagnostics) => 
        _sources[uri] = SyntaxTree.Parse(text, diagnostics);

    public void UpdateAsync(string uri, string text, DiagnosticsProvider diagnostics) => 
        _sources[uri] = SyntaxTree.Parse(text, diagnostics);

    public SyntaxTree GetBufferAsync(string uri) => _sources[uri];

    public void CloseAsync(string uri) => _sources.Remove(uri, out _);
}