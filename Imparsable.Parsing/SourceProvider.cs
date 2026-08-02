namespace Imparsable.Parsing;

public class SourceProvider
{
    private Source? _source;
    public Source Source => _source ?? throw new InvalidOperationException("SourceProvider was not initialized.");

    public void Initialize(string file, string source) => _source = new Source(file, source);
}