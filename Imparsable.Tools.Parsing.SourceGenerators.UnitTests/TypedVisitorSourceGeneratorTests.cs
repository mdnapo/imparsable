using Microsoft.CodeAnalysis;

namespace Imparsable.Tools.Parsing.SourceGenerators.UnitTests;

public class TypedVisitorSourceGeneratorTests
{
    [Fact]
    public void Generator_Correctly_Generates_Typed_Visitor()
    {
        // Arrange
        const string source = """
                              using Imparsable.Tools.Parsing.SourceGenerators.Attributes;

                              namespace Test;

                              [TypedVisitorNode]
                              public partial interface ISyntax
                              {
                              }

                              public partial class NumberSyntax : ISyntax
                              {
                              }
                              """;

        // Act
        var result = GeneratorTest.Run<TypedVisitorSourceGenerator>(source);

        // Assert
        var generated = result.Result.Results
            .Single()
            .GeneratedSources
            .Single(x => x.HintName == "ISyntaxVisitor.g.cs")
            .SourceText
            .ToString();

        Assert.Contains("T Accept<T>(ISyntaxVisitor<T> visitor);", generated);
        Assert.Contains("public interface ISyntaxVisitor<T>", generated);
        Assert.Contains("T Visit(NumberSyntax node);", generated);
    }

    [Fact]
    public void Generator_Produces_Valid_Compilation()
    {
        // Arrange
        const string source = """
                              using Imparsable.Tools.Parsing.SourceGenerators.Attributes;

                              namespace Test;

                              [VoidVisitorNode]
                              public partial interface ISyntax
                              {
                              }

                              public partial class NumberSyntax : ISyntax
                              {
                              }

                              public sealed class Visitor : ISyntaxVisitor
                              {
                                  public void Visit(NumberSyntax node)
                                  {
                                  }
                              }
                              """;

        // Act
        var (_, compilation) = GeneratorTest.Run<VoidVisitorSourceGenerator>(source);

        var diagnostics = compilation
            .GetDiagnostics()
            .Where(x => x.Severity == DiagnosticSeverity.Error)
            .ToArray();

        // Assert
        Assert.Empty(diagnostics);
    }
}