using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Virtualization;
using Imparsable.Toolchain;
using Imparsable.Toolchain.Extensions;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class Calculator : IDisposable
{
    public event Action<Diagnostic> OnDiagnosticPublished = delegate { };
    public event Action<string> OnStdOut = delegate { };
    public event Action<int> OnAllocated = delegate { };
    public event Action<int> OnReclaimed = delegate { };
    public event Action<int> OnCompressed = delegate { };
    public event Action OnExecuted = delegate { };
    public event Action<string> OnDisassemble = delegate { };
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
        vm.Memory.Heap.Allocated += OnAllocated;
        vm.Memory.Heap.Reclaimed += OnReclaimed;
        vm.Memory.Heap.Compressed += OnCompressed;
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
        OnDiagnosticPublished.Clear();
        OnStdOut.Clear();
        OnAllocated.Clear();
        OnReclaimed.Clear();
        OnCompressed.Clear();
        OnExecuted.Clear();
        OnDisassemble.Clear();
        OnDisassembled.Clear();
        OnFailure.Clear();
    }
}