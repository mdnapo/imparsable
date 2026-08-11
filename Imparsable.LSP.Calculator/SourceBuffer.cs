namespace Imparsable.LSP.Calculator;

public class SourceBuffer
{
    private readonly Dictionary<string, string> _sources = new();

    public string this[string path]
    {
        get => _sources[path];
        set => _sources[path] = value;
    }

    public void Remove(string path) => _sources.Remove(path);
}