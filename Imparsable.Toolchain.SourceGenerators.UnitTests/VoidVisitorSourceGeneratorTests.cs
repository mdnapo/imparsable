using Microsoft.CodeAnalysis;

namespace Imparsable.Toolchain.SourceGenerators.UnitTests;

public class VoidVisitorSourceGeneratorTests
{
    [Fact]
    public void Generator_Correctly_Generates_Visitor()
    {
        // Arrange
        const string source = """
                              using Imparsable.Toolchain.SourceGenerators.Attributes;

                              namespace Test;

                              [VoidVisitorNode]
                              public partial interface ISyntax
                              {
                              }

                              public partial class NumberSyntax : ISyntax
                              {
                              }

                              public partial class StringSyntax : ISyntax
                              {
                              }
                              """;

        // Act
        var result = GeneratorTest.Run<VoidVisitorSourceGenerator>(source);

        // Assert
        var generated = result.Result.Results
            .Single()
            .GeneratedSources;

        Assert.Equal(3, generated.Length);
    }

    [Fact]
    public void Generator_Correctly_Generates_Visitor_Interface()
    {
        // Arrange
        const string source = """
                              using Imparsable.Toolchain.SourceGenerators.Attributes;

                              namespace Test;

                              [VoidVisitorNode]
                              public partial interface ISyntax
                              {
                              }

                              public partial class NumberSyntax : ISyntax
                              {
                              }

                              public partial class StringSyntax : ISyntax
                              {
                              }
                              """;

        // Act
        var result = GeneratorTest.Run<VoidVisitorSourceGenerator>(source);

        // Assert
        var generated = result.Result.Results
            .Single()
            .GeneratedSources
            .Single(x => x.HintName == "ISyntaxVisitor.g.cs")
            .SourceText
            .ToString();

        Assert.Contains("public partial interface ISyntax", generated);
        Assert.Contains("void Accept(ISyntaxVisitor visitor);", generated);
        Assert.Contains("public interface ISyntaxVisitor", generated);
        Assert.Contains("void Visit(NumberSyntax node);", generated);
        Assert.Contains("void Visit(StringSyntax node);", generated);
    }

    [Fact]
    public void Generator_Correctly_Generates_Accept_Method()
    {
        // Arrange
        const string source = """
                              using Imparsable.Toolchain.SourceGenerators.Attributes;

                              namespace Test;

                              [VoidVisitorNode]
                              public partial interface ISyntax
                              {
                              }

                              public partial class NumberSyntax : ISyntax
                              {
                              }
                              """;

        // Act
        var result = GeneratorTest.Run<VoidVisitorSourceGenerator>(source);

        // Assert
        var generated = result.Result.Results
            .Single()
            .GeneratedSources
            .Single(x => x.HintName == "NumberSyntax.g.cs")
            .SourceText
            .ToString();

        Assert.Contains("public void Accept(ISyntaxVisitor visitor) => visitor.Visit(this);", generated);
    }

    [Fact]
    public void Generator_Produces_Valid_Compilation()
    {
        // Arrange
        const string source = """
                              using Imparsable.Toolchain.SourceGenerators.Attributes;

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