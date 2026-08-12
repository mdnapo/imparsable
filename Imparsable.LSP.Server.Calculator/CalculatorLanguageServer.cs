using Imparsable.LSP.Protocol;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorLanguageServer(ISourceTextBuffer buffer) : LanguageServer(buffer);