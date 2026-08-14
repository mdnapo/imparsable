using System.Collections;
using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing;

public class DiagnosticsProvider : IEnumerable<Diagnostic>
{
    private List<Diagnostic> Diagnostics { get; } = [];

    public bool IsHealthy => Diagnostics.All(d => d.Severity != DiagnosticSeverity.ERROR);
    
    public void Warning(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.WARNING, marker, message));

    public void Error(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.ERROR, marker, message));

    public IEnumerator<Diagnostic> GetEnumerator() => Diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}