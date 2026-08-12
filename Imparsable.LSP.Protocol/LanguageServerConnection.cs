using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using StreamJsonRpc;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServerConnection<T>(IServiceProvider services) where T : LanguageServer
{
    public async Task RunAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        await using var handler = new WebSocketMessageHandler(socket, new SystemTextJsonFormatter());
        using var rpc = new JsonRpc(handler);
        var server = services.GetRequiredService<T>();

        server.Attach(rpc);
        rpc.AddLocalRpcTarget(server);
        rpc.StartListening();

        await rpc.Completion.WaitAsync(cancellationToken);
    }
}