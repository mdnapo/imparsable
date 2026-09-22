using Projects;

namespace Imparsable.Aspire.Extensions;

public static class DistributedApplicationBuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        internal void AddRunResources()
        {
            var prometheus = builder.AddPrometheus();
            var grafana = builder.AddGrafana(prometheus);
            var jaeger = builder.AddJaeger();
            var openObserve = builder.AddOpenObserve();
            var collector = builder.AddCollector(prometheus, grafana, openObserve, jaeger);

            var api = builder.AddApi()
                .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", collector.GetEndpoint("grpc"))
                .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
                .WaitFor(collector);

            builder
                .AddJavaScriptApp("imparsable-ui", "../Imparsable.UI", "start")
                .WithUrl("http://localhost:4200", "Imparsable UI")
                .WaitFor(api)
                ;
        }

        internal void AddPublishResources()
        {
            var api = builder.AddApi()
                .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
                .PublishWithRegistrySecret();

            var app = builder.AddDockerfile("imparsable-ui", "../Imparsable.UI")
                .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
                .WithExternalHttpEndpoints()
                .PublishWithRegistrySecret();

            var env = builder
                .AddKubernetesEnvironment("imparsable")
                .WithDashboard(enabled: false)
                .WithHelm(helm =>
                {
                    helm.WithChartName("imparsable")
                        .WithNamespace("imparsable")
                        .WithReleaseName("imparsable")
                        .WithChartVersion("1.0.0")
                        .WithChartDescription("The Imparsable Helm chart");
                });

            env.AddIngress("ingress")
                .WithHostname(builder.AddParameter("hostname", "{{ .Values.parameters.ingress.hostname }}"))
                .WithHostname(builder.AddParameter("hostname-www", "{{ .Values.parameters.ingress.hostname_www }}"))
                .WithDefaultBackend(app.GetEndpoint("http"))
                .WithTls();
        }

        private IResourceBuilder<OpenTelemetryCollectorResource> AddCollector(
            IResourceBuilder<ContainerResource> prometheus,
            IResourceBuilder<ContainerResource> grafana,
            IResourceBuilder<ContainerResource> openObserve,
            IResourceBuilder<ContainerResource> jaeger
        )
        {
            return builder
                .AddOpenTelemetryCollector("otel-collector")
                .WithConfig("./observability/otel-collector.yml")
                .WithEnvironment("ASPIRE_OTLP_ENDPOINT", builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"])
                .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp")
                .WithEnvironment("OPENOBSERVE_ENDPOINT", $"{openObserve.GetEndpoint("http")}/api/default")
                .WithEnvironment("OPENOBSERVE_AUTH", "Basic cm9vdEBleGFtcGxlLmNvbTpDb21wbGV4cGFzcyMxMjM=")
                .WaitFor(prometheus)
                .WaitFor(grafana)
                .WaitFor(jaeger)
                .WaitFor(openObserve);
        }

        private IResourceBuilder<ContainerResource> AddOpenObserve()
        {
            return builder
                .AddContainer("openobserve", "o2cr.ai/openobserve/openobserve")
                .WithEnvironment("ZO_ROOT_USER_EMAIL", "root@example.com")
                .WithEnvironment("ZO_ROOT_USER_PASSWORD", "Complexpass#123")
                .WithEnvironment("ZO_DATA_DIR", "/data")
                .WithHttpEndpoint(port: 5080, targetPort: 5080, name: "http")
                .WithUrlForEndpoint("http", url => url.DisplayText = "OpenObserve");
        }

        private IResourceBuilder<ContainerResource> AddJaeger()
        {
            return builder
                .AddContainer("jaeger", "jaegertracing/jaeger")
                .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true")
                .WithHttpEndpoint(port: 16686, targetPort: 16686, name: "http")
                .WithEndpoint(targetPort: 4317, name: "otlp-grpc")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Jaeger");
        }

        private IResourceBuilder<ContainerResource> AddGrafana(IResourceBuilder<ContainerResource> prometheus)
        {
            return builder
                .AddContainer("grafana", "grafana/grafana")
                .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
                .WithBindMount("./observability/grafana/config", "/etc/grafana", isReadOnly: true)
                .WithBindMount("./observability/grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
                .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Grafana")
                .WaitFor(prometheus);
        }

        private IResourceBuilder<ContainerResource> AddPrometheus()
        {
            return builder
                .AddContainer("prometheus", "prom/prometheus")
                .WithBindMount("./observability/prometheus.yml", "/etc/prometheus/prometheus.yml")
                .WithHttpEndpoint(port: 9090, targetPort: 9090, name: "http")
                .WithUrlForEndpoint("http", url => url.DisplayText = "Prometheus");
        }

        private IResourceBuilder<ProjectResource> AddApi() =>
            builder
                .AddProject<Imparsable_API>("imparsable-api")
                .WithUrlForEndpoint("https", url =>
                {
                    url.DisplayText = "Swagger";
                    url.Url += "/swagger";
                });
    }
}