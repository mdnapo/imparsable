namespace Imparsable.Parsing;

public record Diagnostic(DiagnosticSeverity Severity, ISourceMarker Marker, string Message)
{
    public string Report => $"[{Severity}][line: {Marker.Line}, column: {Marker.Column}] {Message}";
}