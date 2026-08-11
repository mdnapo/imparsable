namespace Imparsable.Aspire;

internal sealed record AppResources(
    IResourceBuilder<ProjectResource> Api,
    IResourceBuilder<ProjectResource> CalculatorLsp
);