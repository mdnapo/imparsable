using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class SyntaxTree : SyntaxTree<Token, ISyntax, SyntaxTree>
{
    public SymbolTable SymbolTable { get; } = new();
    public Dictionary<ISyntax, string> TypeMap { get; set; } = [];

    public static SyntaxTree Parse(string source)
    {
        var tree = Parse<Statement, Statement.Synchronizer>(source);
        SymbolResolver.Execute(tree);
        TypeResolver.Execute(tree);
        return tree;
    }
}