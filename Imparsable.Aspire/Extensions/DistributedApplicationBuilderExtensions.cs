using Projects;

namespace Imparsable.Aspire.Extensions;

public static class DistributedApplicationBuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        internal AppResources AddSharedResources()
        {
            var api = builder
                .AddProject<Imparsable_API>("imparsable-api")
                .WithUrlForEndpoint("https", url =>
                {
                    url.DisplayText = "Swagger";
                    url.Url += "/swagger";
                });

            return new AppResources(api);
        }

        internal void AddRunResources(AppResources resources)
        {
            builder
                .AddJavaScriptApp("imparsable-ui", "../Imparsable.UI", "start")
                .WithUrl("http://localhost:4200", "UI")
                .WaitFor(resources.Api);
        }
    }
}