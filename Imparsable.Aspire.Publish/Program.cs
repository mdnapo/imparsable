using Imparsable.Aspire.Publish;

await Publisher.PublishAsync();

await YamlFile.ModifyAsync(Publisher.ValuesFilePath, values =>
{
    values.Set("parameters.infisical.host", "https://infisical.example.com");
    values.Set("parameters.infisical.env", "dev");
    values.Set("parameters.infisical.auth_secret_name", "infisical-universal-auth-credentials");
    values.Set("parameters.infisical.auth_secret_namespace", "infisical");
    values.Set("parameters.registry.project", "image-pull-secret");
    values.Set("parameters.ingress.hostname", "example.com");
    values.Set("parameters.ingress.hostname_www", "www.example.com");
});