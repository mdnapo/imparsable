using Aspire.Hosting.Kubernetes.Resources;

namespace Imparsable.Aspire.Extensions;

public static class ResourceBuilderExtensions
{
    internal static IResourceBuilder<T> PublishWithRegistrySecret<T>(this IResourceBuilder<T> builder)
        where T : IComputeResource =>
        builder.PublishAsKubernetesService(resource =>
        {
            if (resource.Workload is not Deployment deployment) return;

            var imagePullSecrets = deployment.Spec.Template.Spec.ImagePullSecrets;
            imagePullSecrets.Add(new LocalObjectReferenceV1 { Name = "registry-secret" });
        });
}