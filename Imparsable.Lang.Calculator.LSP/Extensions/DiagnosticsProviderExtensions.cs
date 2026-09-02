using Imparsable.Toolchain;
using Imparsable.Toolchain.Parsing.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Diagnostic = OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using ImparsableDiagnosticSeverity = Imparsable.Toolchain.DiagnosticSeverity;
using OmnisharpDiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Imparsable.Lang.Calculator.LSP.Extensions;

public static class DiagnosticsProviderExtensions
{
    public static PublishDiagnosticsParams ToPublishDiagnosticsParams(this DiagnosticsProvider diagnostics, string uri) => new()
    {
        Uri = uri,
        Diagnostics = new Container<Diagnostic>(diagnostics.Select(diagnostic => new Diagnostic
        {
            Range = diagnostic.Marker.ToRange(),
            Severity = diagnostic.Severity switch
            {
                ImparsableDiagnosticSeverity.WARNING => OmnisharpDiagnosticSeverity.Warning,
                ImparsableDiagnosticSeverity.ERROR => OmnisharpDiagnosticSeverity.Error,
                _ => throw new InvalidOperationException()
            },
            Message = diagnostic.Message,
        }))
    };

    private static Range ToRange(this ISourceMarker marker) => new(
        startLine: marker.Line - 1,
        startCharacter: marker.Column - 1,
        endLine: marker.Line - 1,
        endCharacter: marker.Column + marker.Length - 1
    );
}