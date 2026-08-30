using System.Collections;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain;

public class DiagnosticsProvider : IEnumerable<Diagnostic>, IDisposable
{
    private List<Diagnostic> Diagnostics { get; } = [];
    public event Action<Diagnostic> Published = delegate { };
    public bool IsHealthy => Diagnostics.All(d => d.Severity != DiagnosticSeverity.ERROR);

    public IEnumerator<Diagnostic> GetEnumerator() => Diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Warning(ISourceMarker marker, string message)
    {
        var diagnostic = new Diagnostic(DiagnosticSeverity.WARNING, marker, message);
        Diagnostics.Add(diagnostic);
        Published.Invoke(diagnostic);
    }

    public void Error(ISourceMarker marker, string message)
    {
        var diagnostic = new Diagnostic(DiagnosticSeverity.ERROR, marker, message);
        Diagnostics.Add(diagnostic);
        Published.Invoke(diagnostic);
    }

    public T Halt<T>(ISourceMarker marker, string message) where T : Exception
    {
        Error(marker, message);
        return (T)Activator.CreateInstance(typeof(T), message)!;
    }

    public void Dispose()
    {
        foreach (var @delegate in Published.GetInvocationList())
            Published -= @delegate as Action<Diagnostic>;
    }
}