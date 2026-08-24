using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imparsable.Tools.Parsing.SourceGenerators;

public abstract partial class VisitorSourceGenerator
{
    private class Walker(string interfaceName) : CSharpSyntaxWalker
    {
        public List<ClassDeclarationSyntax> ImplementingTypes { get; } = [];

        private string InterfaceName { get; } = interfaceName;

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.BaseList?.Types.Count > 0)
            {
                foreach (var item in node.BaseList.Types)
                {
                    if (item.Type is IdentifierNameSyntax ins && ins.ToFullString().Trim() == InterfaceName)
                    {
                        ImplementingTypes.Add(node);
                    }
                }
            }
        }
    }
}