namespace Imparsable.LSP.Protocol;

public static class LspMethodName
{
    public const string Initialize = "initialize";
    public const string Initialized = "initialized";
    public const string TextDocumentDidOpen = "textDocument/didOpen";
    public const string TextDocumentDidChange = "textDocument/didChange";
    public const string TextDocumentDidClose = "textDocument/didClose";
    public const string TextDocumentCompletion = "textDocument/completion";
    
    public const string PublishDiagnostics = "textDocument/publishDiagnostics";
}