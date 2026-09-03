using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidChangeHandler(SyntaxBuffer buffer, JsonRpcProvider rpc) : ITextDocumentDidChangeHandler
{
    public async Task HandleAsync(DidChangeTextDocumentParams parameters)
    {
        var diagnostics = new DiagnosticsProvider();
        var uri = parameters.TextDocument.Uri.ToString();
        var source = buffer.GetBufferAsync(uri).Source.Text;
        
        foreach (var change in parameters.ContentChanges)
            source = ApplyChange(source, change);

        buffer.UpdateAsync(uri, source, diagnostics);

        var publishDiagnosticsParams = diagnostics.ToPublishDiagnosticsParams(uri);

        await rpc.Connection.NotifyWithParameterObjectAsync(LspMethodName.PublishDiagnostics, publishDiagnosticsParams);
    }

    private static string ApplyChange(string text, TextDocumentContentChangeEvent change)
    {
        if (change.Range is null)
            return change.Text;

        var start = GetOffset(text, change.Range.Start);
        var end = GetOffset(text, change.Range.End);
        return string.Concat(text.AsSpan(0, start), change.Text, text.AsSpan(end));
    }

    private static int GetOffset(string text, Position position)
    {
        var line = 0;
        var offset = 0;

        while (line < position.Line && offset < text.Length)
        {
            if (text[offset] == '\n')
                line++;

            offset++;
        }

        return offset + position.Character;
    }
}