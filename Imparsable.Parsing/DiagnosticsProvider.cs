using System.Collections;

namespace Imparsable.Parsing;

public class DiagnosticsProvider : IEnumerable<Diagnostic>
{
    private List<Diagnostic> Diagnostics { get; set; } = [];

    public bool IsHealthy => Diagnostics.All(d => d.Severity != DiagnosticSeverity.Error);
    
    public void Warning(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Warning, marker, message));

    public void Error(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Error, marker, message));

    public IEnumerator<Diagnostic> GetEnumerator() => Diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}