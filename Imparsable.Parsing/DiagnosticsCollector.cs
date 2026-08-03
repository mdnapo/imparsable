namespace Imparsable.Parsing;

public class DiagnosticsCollector 
{
    public List<Diagnostic> Diagnostics { get; set; } = [];

    public void Warning(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Warning, marker, message));

    public void Error(ISourceMarker marker, string message) =>
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Error, marker, message));
}