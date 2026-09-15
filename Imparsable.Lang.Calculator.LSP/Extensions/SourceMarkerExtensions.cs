using Imparsable.Toolchain.Parsing.Interfaces;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Imparsable.Lang.Calculator.LSP.Extensions;

public static class SourceMarkerExtensions
{
    public static Range ToRange(this ISourceMarker marker) => new(
        startLine: marker.Line - 1,
        startCharacter: marker.Column - 1,
        endLine: marker.Line - 1,
        endCharacter: marker.Column + marker.Length - 1
    );
}