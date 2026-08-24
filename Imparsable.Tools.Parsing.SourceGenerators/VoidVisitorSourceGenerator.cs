using Microsoft.CodeAnalysis;

namespace Imparsable.Tools.Parsing.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class VoidVisitorSourceGenerator : VisitorSourceGenerator
{
    protected override string GetVisitorMetadataAttribute =>
        "Imparsable.Tools.Parsing.SourceGenerators.Attributes.VoidVisitorNodeAttribute";

    protected override string GetVisitorInterfaceTypeName => "Visitor";
    protected override string GetVisitorInterfaceAcceptSignature => "Accept";
    protected override string GetVisitorInterfaceReturnType => "void";
}