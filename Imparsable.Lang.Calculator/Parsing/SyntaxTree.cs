using Imparsable.Toolchain;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public class SyntaxTree : SyntaxTree<Token, Interfaces.ISyntax, SyntaxTree>
{
    public SymbolRoot SymbolRoot { get; } = [];
    public Dictionary<Interfaces.ISyntax, SystemType> Types { get; } = [];

    public static SyntaxTree Parse(string source, DiagnosticsProvider diagnostics)
    {
        var tree = Parse<Statement, Statement.Synchronizer>(source, diagnostics);
        ScopeResolver.Execute(tree, diagnostics);
        TypeResolver.Execute(tree, diagnostics);
        return tree;
    }
}