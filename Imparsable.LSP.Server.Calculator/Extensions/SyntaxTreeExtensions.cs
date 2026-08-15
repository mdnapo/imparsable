using Imparsable.Parsing.Interfaces;
using Imparsable.Tool.Calculator.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using ImparsableDiagnosticSeverity = Imparsable.Parsing.DiagnosticSeverity;
using OmnisharpDiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Imparsable.LSP.Server.Calculator.Extensions;

public static class SyntaxTreeExtensions
{
    public static PublishDiagnosticsParams ToPublishDiagnosticsParams(this SyntaxTree tree, string uri) => new()
    {
        Uri = uri,
        Diagnostics = new Container<Diagnostic>(tree.Diagnostics.Select(diagnostic => new Diagnostic
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