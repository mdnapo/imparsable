using Microsoft.CodeAnalysis;

namespace Imparsable.Parsing.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class TypedVisitorSourceGenerator : VisitorSourceGenerator
{
    protected override string GetVisitorMetadataAttribute =>
        "Imparsable.Parsing.SourceGenerators.Attributes.TypedVisitorNodeAttribute";

    protected override string GetVisitorInterfaceTypeName => "Visitor<T>";
    protected override string GetVisitorInterfaceAcceptSignature => "Accept<T>";
    protected override string GetVisitorInterfaceReturnType => "T";
}