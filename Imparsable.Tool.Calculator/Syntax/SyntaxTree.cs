using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class SyntaxTree : SyntaxTree<Token, ISyntax, SyntaxTree>
{
    public static SyntaxTree Parse(string source)
    {
        var tree = Parse<Statement.Production, Statement.Synchronizer>(source);
        SymbolResolver.Execute(tree);
        TypeResolver.Execute(tree);
        return tree;
    }
}