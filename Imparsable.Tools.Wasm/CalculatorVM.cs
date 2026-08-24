using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Virtualization;
using Imparsable.Tools.Parsing;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class CalculatorVM : IDisposable
{
    public event Action<Diagnostic> OnDiagnosticPublished = delegate { };
    public event Action<string> OnStdOut = delegate { };

    public void Execute(string code)
    {
        using var tree = SyntaxTree.Parse(code, OnDiagnosticPublished);

        if (!tree.IsHealthy) return;

        var chunk = Compiler.Execute(tree);
        using var vm = new VirtualMachine();
        vm.StdOut += OnStdOut;
        vm.Execute(chunk);
    }

    public void Dispose()
    {
        foreach (var @delegate in OnDiagnosticPublished.GetInvocationList())
            OnDiagnosticPublished -= @delegate as Action<Diagnostic>;

        foreach (var @delegate in OnStdOut.GetInvocationList())
            OnStdOut -= @delegate as Action<string>;
    }
}