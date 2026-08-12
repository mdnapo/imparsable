using Imparsable.LSP.Protocol;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorLanguageServerConnection(IServiceProvider services) :
    LanguageServerConnection<CalculatorLanguageServer>(services);