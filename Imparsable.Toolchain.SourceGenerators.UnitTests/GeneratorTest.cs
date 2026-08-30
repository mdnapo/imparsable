using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Imparsable.Toolchain.SourceGenerators.UnitTests;

public static class GeneratorTest
{
    public record struct Output(GeneratorDriverRunResult Result, Compilation Compilation);

    public static Output Run<TGenerator>(string source) where TGenerator : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new TGenerator().AsSourceGenerator());

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        return new Output(driver.GetRunResult(), outputCompilation);
    }
}