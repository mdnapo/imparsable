using System.Diagnostics;
using System.Reflection;
using Imparsable.Aspire.Publish.Extensions;

namespace Imparsable.Aspire.Publish;

internal static class Publisher
{
    internal static readonly string HelmDir = Path.GetFullPath("helm", Environment.CurrentDirectory);
    internal static readonly string SecretsDir = Path.Combine(HelmDir, "templates", "secrets");
    internal static readonly string NamespaceDir = Path.Combine(HelmDir, "templates", "namespace");
    internal static readonly string ValuesFilePath = Path.Combine(HelmDir, "values.yaml");
    internal static readonly string RegistrySecretsDestination = Path.Combine(SecretsDir, "registry-secret.yaml");
    internal static readonly string RegistrySecretsResource = ReadResource("resources.registry-secret.yaml");
    internal static readonly string NamespaceDestination = Path.Combine(NamespaceDir, "namespace.yaml");
    internal static readonly string NamespaceResource = ReadResource("resources.namespace.yaml");

    internal static async Task PublishAsync()
    {
        Directory.DeleteIfExists(HelmDir);

        await RunAsync("aspire", "publish", "-o", HelmDir);

        await RunAsync("mkdir", "-p", SecretsDir);
        await File.WriteAllTextAsync(RegistrySecretsDestination, RegistrySecretsResource);

        await RunAsync("mkdir", "-p", NamespaceDir);
        await File.WriteAllTextAsync(NamespaceDestination, NamespaceResource);
    }

    internal static async Task RunAsync(string command, params string[] arguments)
    {
        var startInfo = new ProcessStartInfo(command)
        {
            UseShellExecute = false
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo) ??
                            throw new InvalidOperationException($"Failed to start '{command}'.");

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"'{command}' exited with code {process.ExitCode}.");
        }
    }

    internal static string ReadResource(string resourceName)
    {
        resourceName = resourceName.Replace(Path.PathSeparator, '.');

        var assembly = Assembly.GetExecutingAssembly();
        var @namespace = assembly.GetName().Name ?? throw new InvalidOperationException();
        using var stream = assembly.GetManifestResourceStream($"{@namespace}.{resourceName}");
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
        return reader.ReadToEnd();
    }
}