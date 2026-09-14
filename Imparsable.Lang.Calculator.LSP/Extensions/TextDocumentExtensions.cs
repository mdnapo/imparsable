using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP.Extensions;

internal static class TextDocumentExtensions
{
    internal static bool IsCalculatorDocument(this TextDocumentItem document) =>
        document is { Uri.Scheme: Constants.FileScheme, LanguageId: Constants.LanguageId } &&
        document.Uri.Path.EndsWith(Constants.FileExtension, StringComparison.OrdinalIgnoreCase);

    internal static bool IsCalculatorDocument(this TextDocumentIdentifier document) =>
        document is { Uri.Scheme: Constants.FileScheme } &&
        document.Uri.Path.EndsWith(Constants.FileExtension, StringComparison.OrdinalIgnoreCase);
}