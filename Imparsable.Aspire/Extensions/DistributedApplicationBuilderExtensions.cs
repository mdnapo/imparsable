using Projects;

namespace Imparsable.Aspire.Extensions;

public static class DistributedApplicationBuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        internal AppResources AddSharedResources()
        {
            var prometheus = builder
                .AddContainer("prometheus", "prom/prometheus")
                .WithBindMount("./observability/prometheus.yml", "/etc/prometheus/prometheus.yml")
                .WithHttpEndpoint(port: 9090, targetPort: 9090, name: "http")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Prometheus");

            builder
                .AddContainer("grafana", "grafana/grafana")
                .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
                .WithBindMount("./observability/grafana/config", "/etc/grafana", isReadOnly: true)
                .WithBindMount("./observability/grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
                .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Grafana")
                .WaitFor(prometheus);

            var jaeger = builder
                .AddContainer("jaeger", "jaegertracing/jaeger")
                .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true")
                .WithHttpEndpoint(port: 16686, targetPort: 16686, name: "http")
                .WithEndpoint(targetPort: 4317, name: "otlp-grpc")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Jaeger");

            var collector = builder
                .AddOpenTelemetryCollector("otel-collector")
                .WithConfig("./observability/otel-collector.yml")
                .WithEnvironment("ASPIRE_OTLP_ENDPOINT", builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"])
                .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp")
                .WaitFor(prometheus)
                .WaitFor(jaeger);

            var api = builder
                .AddProject<Imparsable_API>("imparsable-api")
                .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", collector.GetEndpoint("grpc"))
                .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
                .WaitFor(collector)
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
                .WithUrl("http://localhost:4200", "Imparsable UI")
                .WaitFor(resources.Api)
                ;
        }
    }
}