using System.Diagnostics;

namespace Imparsable.Toolchain.LSP;

internal static class LspTelemetry
{
    public const string SourceName = "Imparsable.Toolchain.LSP";

    private static readonly ActivitySource Source = new(SourceName);

    public static void Invoke(string method, string connectionId, Action action)
    {
        using var activity = StartActivity(method, connectionId);

        try
        {
            action();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    public static TResult Invoke<TResult>(string method, string connectionId, Func<TResult> action)
    {
        using var activity = StartActivity(method, connectionId);

        try
        {
            var result = action();

            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    public static async Task InvokeAsync(string method, string connectionId, Func<Task> action)
    {
        using var activity = StartActivity(method, connectionId);

        try
        {
            await action();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    private static Activity? StartActivity(string method, string connectionId)
    {
        // The default ActivityContext is necessary to make each operation
        // exportable independantly from the trace started by the Controller request.
        var activity = Source.CreateActivity(method, ActivityKind.Server, default(ActivityContext));

        activity?.SetTag("rpc.system", "jsonrpc");
        activity?.SetTag("rpc.method", method);
        activity?.SetTag("lsp.connection.id", connectionId);
        activity?.Start();

        return activity;
    }

    private static void RecordException(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity?.AddException(exception);
    }
}