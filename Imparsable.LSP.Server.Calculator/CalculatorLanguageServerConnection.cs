using Imparsable.LSP.Protocol;
using Microsoft.AspNetCore.Http;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorLanguageServerConnection(
    IHttpContextAccessor httpContextAccessor,
    CalculatorLanguageServer server
) : LanguageServerConnection<CalculatorLanguageServer>(httpContextAccessor, server);