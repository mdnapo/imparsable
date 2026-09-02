using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Virtualization;
using Imparsable.Toolchain;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class Calculator : IDisposable
{
    public event Action<Diagnostic> OnDiagnosticPublished = delegate { };
    public event Action<string> OnStdOut = delegate { };
    public event Action<string> OnDisassemble = delegate { };

    public void Execute(string code)
    {
        using var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += OnDiagnosticPublished;
        var tree = SyntaxTree.Parse(code, diagnostics);

        if (!diagnostics.IsHealthy || Compiler.Compile(tree, diagnostics) is not { } chunk) return;

        using var vm = new VirtualMachine();
        vm.StdOut += OnStdOut;
        vm.Execute(chunk);
    }

    public void Disassemble(string code)
    {
        using var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += OnDiagnosticPublished;
        var tree = SyntaxTree.Parse(code, diagnostics);

        if (!diagnostics.IsHealthy || Disassembler.Disassemble(tree, diagnostics) is not { } output) return;

        OnDisassemble.Invoke(output);
    }

    public void Dispose()
    {
        foreach (var @delegate in OnDiagnosticPublished.GetInvocationList())
            OnDiagnosticPublished -= @delegate as Action<Diagnostic>;

        foreach (var @delegate in OnStdOut.GetInvocationList())
            OnStdOut -= @delegate as Action<string>;

        foreach (var @delegate in OnDisassemble.GetInvocationList())
            OnStdOut -= @delegate as Action<string>;
    }
}