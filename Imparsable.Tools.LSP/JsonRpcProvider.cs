using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Imparsable.Tools.LSP;

public sealed class JsonRpcProvider(IHttpContextAccessor httpContextAccessor) : IAsyncDisposable
{
    private WebSocket? _socket;
    private WebSocketMessageHandler? _handler;
    private JsonRpc? _connection;

    private HttpContext HttpContext => httpContextAccessor.HttpContext;
    public JsonRpc Connection => _connection ?? throw new InvalidOperationException();

    public async Task ConnectAsync(object target)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        _socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        _handler = new WebSocketMessageHandler(_socket, GetFormatter());
        _connection = new JsonRpc(_handler);
        _connection.AddLocalRpcTarget(target);
        _connection.StartListening();

        await _connection.Completion.WaitAsync(HttpContext.RequestAborted);
    }

    private static JsonMessageFormatter GetFormatter() => new()
    {
        JsonSerializer =
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }
    };

    public async ValueTask DisposeAsync()
    {
        if (_socket != null) await CastAndDispose(_socket);
        if (_handler != null) await CastAndDispose(_handler);
        if (_connection != null) await CastAndDispose(_connection);

        return;

        static async ValueTask CastAndDispose(IDisposable disposable)
        {
            if (disposable is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                disposable.Dispose();
        }
    }
}