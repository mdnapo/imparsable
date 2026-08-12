using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServerConnection<TServer>(
    IHttpContextAccessor httpContextAccessor,
    TServer server
) where TServer : LanguageServer
{
    public async Task RunAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (!httpContext.WebSockets.IsWebSocketRequest)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

        var fmt = new JsonMessageFormatter
        {
            JsonSerializer =
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };
        
        // var fmt = new SystemTextJsonFormatter();
        await using var handler = new WebSocketMessageHandler(webSocket, fmt);
        using var rpc = new JsonRpc(handler);
        // var server = services.GetRequiredService<T>();

        server.Attach(rpc);
        rpc.AddLocalRpcTarget(server);
        rpc.StartListening();

        await rpc.Completion.WaitAsync(httpContext.RequestAborted);
    }
}