using Imparsable.Aspire.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

switch (builder.ExecutionContext.Operation)
{
    case DistributedApplicationOperation.Run:
        builder.AddRunResources();
        break;

    case DistributedApplicationOperation.Publish:
        builder.AddPublishResources();
        break;

    default:
        throw new InvalidOperationException("Unknown operation: " + builder.ExecutionContext.Operation);
}

await builder
    .Build()
    .RunAsync();