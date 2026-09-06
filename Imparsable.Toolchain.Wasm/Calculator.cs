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
    public event Action OnExecuted = delegate { };
    public event Action OnDisassembled = delegate { };
    public event Action OnFailure = delegate { };

    public void Execute(string code)
    {
        using var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += OnDiagnosticPublished;
        var tree = SyntaxTree.Parse(code, diagnostics);

        if (!diagnostics.IsHealthy || Compiler.Compile(tree, diagnostics) is not { } chunk)
        {
            OnFailure.Invoke();
            return;
        }

        using var vm = new VirtualMachine();
        vm.StdOut += OnStdOut;
        vm.Execute(chunk);
        OnExecuted.Invoke();
    }

    public void Disassemble(string code)
    {
        using var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += OnDiagnosticPublished;
        var tree = SyntaxTree.Parse(code, diagnostics);

        if (!diagnostics.IsHealthy || Disassembler.Disassemble(tree, diagnostics) is not { } output)
        {
            OnFailure.Invoke();
            return;
        }

        OnDisassemble.Invoke(output);
        OnDisassembled.Invoke();
    }

    public void Dispose()
    {
        foreach (var @delegate in OnDiagnosticPublished.GetInvocationList())
            OnDiagnosticPublished -= @delegate as Action<Diagnostic>;

        foreach (var @delegate in OnStdOut.GetInvocationList())
            OnStdOut -= @delegate as Action<string>;

        foreach (var @delegate in OnDisassemble.GetInvocationList())
            OnDisassemble -= @delegate as Action<string>;

        foreach (var @delegate in OnExecuted.GetInvocationList())
            OnExecuted -= @delegate as Action;

        foreach (var @delegate in OnDisassembled.GetInvocationList())
            OnDisassembled -= @delegate as Action;

        foreach (var @delegate in OnFailure.GetInvocationList())
            OnFailure -= @delegate as Action;
    }
}