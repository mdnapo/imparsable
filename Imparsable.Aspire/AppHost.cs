using Imparsable.Aspire.Extensions;

var builder = DistributedApplication.CreateBuilder(args);
var resources = builder.AddSharedResources();

// if (builder.ExecutionContext.IsPublishMode)
// {
//     builder.AddPublishResources(resources);
// }
// else
// {
    builder.AddRunResources(resources);
// }

await builder
    .Build()
    .RunAsync();