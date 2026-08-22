using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Execution;
using Imparsable.Tool.Calculator.Syntax;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class CalculatorVM
{
    public event Action<string>? Out;

    public void Execute(string code)
    {
        if (SyntaxTree.Parse(code) is not { } tree || !Validate(tree)) return;

        var chunk = Compiler.Execute(tree);
        using var vm = new VirtualMachine();
        vm.Out += Out;
        vm.Execute(chunk);
    }

    private bool Validate(SyntaxTree tree)
    {
        foreach (var diagnostic in tree.Diagnostics)
            Out?.Invoke(diagnostic.Report);

        return tree.Diagnostics.IsHealthy;
    }
}