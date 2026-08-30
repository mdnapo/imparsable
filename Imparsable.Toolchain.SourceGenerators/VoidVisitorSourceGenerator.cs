using Microsoft.CodeAnalysis;

namespace Imparsable.Toolchain.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class VoidVisitorSourceGenerator : VisitorSourceGenerator
{
    protected override string GetVisitorMetadataAttribute =>
        "Imparsable.Toolchain.SourceGenerators.Attributes.VoidVisitorNodeAttribute";

    protected override string GetVisitorInterfaceTypeName => "Visitor";
    protected override string GetVisitorInterfaceAcceptSignature => "Accept";
    protected override string GetVisitorInterfaceReturnType => "void";
}