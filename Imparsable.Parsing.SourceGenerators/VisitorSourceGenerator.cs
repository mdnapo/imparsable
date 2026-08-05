using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imparsable.Parsing.SourceGenerators;

public abstract partial class VisitorSourceGenerator : IIncrementalGenerator
{
    protected abstract string GetVisitorMetadataAttribute { get; } // "Full.Path.To.Attribute"
    protected abstract string GetVisitorInterfaceTypeName { get; } // Visitor or Visitor<T>
    protected abstract string GetVisitorInterfaceAcceptSignature { get; } // Accept or Accept<T>
    protected abstract string GetVisitorInterfaceReturnType { get; } // void or T

    private void AddVisitorInterface(
        Walker walker,
        StringBuilder sb,
        bool indentInterface,
        string interfaceName
    )
    {
        IndentCurrentLineIfRequired(indentInterface, sb);
        sb.Append("public interface ");
        sb.AppendLine(interfaceName);
        IndentCurrentLineIfRequired(indentInterface, sb);
        sb.AppendLine("{");
        foreach (var t in walker.ImplementingTypes)
        {
            IndentCurrentLineIfRequired(indentInterface, sb);
            sb.Append($"    {GetVisitorInterfaceReturnType} Visit(");
            sb.Append(t.Identifier.ToFullString().TrimEnd());
            sb.AppendLine(" node);");
        }

        IndentCurrentLineIfRequired(indentInterface, sb);
        sb.AppendLine("}");
    }

    private static void IndentCurrentLineIfRequired(bool indent, StringBuilder nodeStringBuilder)
    {
        if (indent)
        {
            nodeStringBuilder.Append("    ");
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<InterfaceDeclarationSyntax?> classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GetVisitorMetadataAttribute,
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(m => m != null);

        IncrementalValueProvider<(Compilation Left, ImmutableArray<InterfaceDeclarationSyntax?> Right)>
            compilationAndClasses =
                context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private void Execute(
        Compilation compilation,
        ImmutableArray<InterfaceDeclarationSyntax?> classes,
        SourceProductionContext context
    )
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var interfaces = classes.Distinct();

        foreach (var item in interfaces)
        {
            if (item is null)
            {
                continue;
            }

            var interfaceName = item.Identifier.ToFullString().Trim();
            var walker = new Walker(interfaceName);

            var interfaceSemanticModel = compilation.GetSemanticModel(item.SyntaxTree);
            var interfaceSymbol = ModelExtensions.GetDeclaredSymbol(interfaceSemanticModel, item);

            if (interfaceSymbol is null)
            {
                continue;
            }

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return;
                var root = syntaxTree.GetCompilationUnitRoot(context.CancellationToken);

                walker.Visit(root);
            }

            if (walker.ImplementingTypes.Count == 0)
            {
                return;
            }

            var sb = new StringBuilder();

            var visitorName = interfaceName + GetVisitorInterfaceTypeName;

            foreach (var t in walker.ImplementingTypes)
            {
                var nodeSemanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                var nodeSymbol = ModelExtensions.GetDeclaredSymbol(nodeSemanticModel, t);

                if (nodeSymbol is null)
                {
                    continue;
                }

                var name = t.Identifier.ToFullString().Trim();
                var nodeSb = new StringBuilder();
                var indent = false;

                if (!nodeSymbol.ContainingNamespace.IsGlobalNamespace)
                {
                    indent = true;
                    nodeSb.Append("namespace ");
                    nodeSb.AppendLine(nodeSymbol.ContainingNamespace.ToString());
                    nodeSb.AppendLine("{");
                }

                IndentCurrentLineIfRequired(indent, nodeSb);
                nodeSb.Append("public partial class ");
                nodeSb.AppendLine(name);
                IndentCurrentLineIfRequired(indent, nodeSb);
                nodeSb.AppendLine("{");
                nodeSb.Append($"    public {GetVisitorInterfaceReturnType} {GetVisitorInterfaceAcceptSignature}(")
                    .Append(visitorName)
                    .AppendLine(" visitor) => visitor.Visit(this);");

                IndentCurrentLineIfRequired(indent, nodeSb);
                nodeSb.Append('}');

                if (!nodeSymbol.ContainingNamespace.IsGlobalNamespace)
                {
                    nodeSb.AppendLine("").Append("}");
                }

                context.AddSource(name + ".g.cs", nodeSb.ToString());

                if (!nodeSymbol.ContainingNamespace.Equals(interfaceSymbol.ContainingNamespace,
                        SymbolEqualityComparer.Default))
                {
                    sb.Append("using ").Append(nodeSymbol.ContainingNamespace).AppendLine(";");
                }
            }

            var indentInterface = false;

            if (!interfaceSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                indentInterface = true;
                sb.Append("namespace ");
                sb.AppendLine(interfaceSymbol.ContainingNamespace.ToString());
                sb.AppendLine("{");
            }

            IndentCurrentLineIfRequired(indentInterface, sb);
            sb.Append("public partial interface ").AppendLine(interfaceName);
            IndentCurrentLineIfRequired(indentInterface, sb);
            sb.AppendLine("{");
            IndentCurrentLineIfRequired(indentInterface, sb);
            sb.Append($"    {GetVisitorInterfaceReturnType} {GetVisitorInterfaceAcceptSignature}(")
                .Append(visitorName)
                .AppendLine(" visitor);");
            IndentCurrentLineIfRequired(indentInterface, sb);
            sb.AppendLine("}");

            AddVisitorInterface(walker, sb, indentInterface, visitorName);

            if (!interfaceSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                sb.AppendLine("").Append('}');
            }

            context.AddSource(interfaceName + "Visitor.g.cs", sb.ToString());
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode) =>
        syntaxNode is InterfaceDeclarationSyntax;

    static InterfaceDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context) =>
        context.TargetNode as InterfaceDeclarationSyntax;
}