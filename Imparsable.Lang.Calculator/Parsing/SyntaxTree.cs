using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public class SyntaxTree : SyntaxTree<Token, ISyntax, SyntaxTree>
{
    public SymbolTable SymbolTable { get; } = new();
    public Dictionary<ISyntax, SystemType> Types { get; } = [];

    public static SyntaxTree Parse(string source, Action<Diagnostic>? diagnosticHandler = null)
    {
        var tree = Parse<Statement, Statement.Synchronizer>(source, diagnosticHandler);
        SymbolResolver.Execute(tree);
        TypeResolver.Execute(tree);
        return tree;
    }
}