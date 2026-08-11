using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Calculator;

public static class Defaults
{
    public static readonly TextDocumentSelector DocumentSelector = new(new TextDocumentFilter { Pattern = "**/*.clc" });
}

