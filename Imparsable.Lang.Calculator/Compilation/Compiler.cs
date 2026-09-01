using Imparsable.Lang.Calculator.Exceptions;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;
using Imparsable.Toolchain.Compilation;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed class Compiler(SyntaxTree tree, DiagnosticsProvider diagnostics) : CompilerBase(tree, diagnostics)
{
    public static Chunk? Execute(SyntaxTree tree, DiagnosticsProvider diagnostics) =>
        new Compiler(tree, diagnostics).Execute();

    public Chunk? Execute()
    {
        try
        {
            Tree.SymbolRoot.Popped += OnPop;

            foreach (var node in Tree.Roots)
                node.Accept(this);

            return Build();
        }
        catch (HaltException)
        {
            return null;
        }
        finally
        {
            Tree.SymbolRoot.Popped -= OnPop;
        }
    }
}