namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public class ContextProvider(Parser<TToken>.Configuration configuration, DiagnosticsProvider diagnostics)
    {
        private Source? _source;
        public Source Source => _source ?? throw new InvalidOperationException("Source was not initialized.");

        public void Initialize(string file, string source) => _source = new Source(file, source);
        public Context GetContext() => new(configuration, Source, diagnostics);
    }
}