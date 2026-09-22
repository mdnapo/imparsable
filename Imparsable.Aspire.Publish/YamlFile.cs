using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Imparsable.Aspire.Publish;

internal class YamlFile
{
    private readonly YamlMappingNode _root;
    private readonly string _path;

    private YamlFile(string path)
    {
        using var reader = File.OpenText(path);
        var stream = new YamlStream();
        stream.Load(reader);
        _root = stream.Documents.First().RootNode as YamlMappingNode ?? throw new InvalidCastException();
        _path = path;
    }

    internal static async Task ModifyAsync(string path, Action<YamlFile> action)
    {
        var file = new YamlFile(path);
        action(file);
        await file.SaveAsync();
    }

    internal void Set(string path, string value)
    {
        var parts = path.Split('.');
        var current = _root;

        for (var i = 0; i < parts.Length - 1; i++)
        {
            var key = new YamlScalarNode(parts[i]);

            if (!current.Children.TryGetValue(key, out var child))
            {
                child = new YamlMappingNode();
                current.Add(key, child);
            }

            current = child as YamlMappingNode ??
                      throw new InvalidOperationException($"'{string.Join('.', parts[..(i + 1)])}' is not a mapping.");
        }

        current.Children[new YamlScalarNode(parts[^1])] = new YamlScalarNode(value);
    }

    private async Task SaveAsync()
    {
        var serializer = new Serializer();
        await using var file = File.OpenWrite(_path);
        await using var writer = new StreamWriter(file);
        serializer.Serialize(writer, _root);
    }
}