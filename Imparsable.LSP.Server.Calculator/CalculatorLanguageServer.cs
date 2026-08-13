using Imparsable.LSP.Protocol;
using Microsoft.AspNetCore.Http;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorLanguageServer(IHttpContextAccessor httpContextAccessor, ISourceTextBuffer buffer)
    : LanguageServer(httpContextAccessor, buffer);