using System.CommandLine;
using Imparsable.CLI.Interfaces;
using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Execution;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.CLI.Commands;

internal sealed partial class Imp
{
    internal sealed partial class Calculator
    {
        internal sealed class Run : Command, ISubCommandOf<Calculator>
        {
            public Run() : base("run", "Run Imparsable calculator commands")
            {
                Options.Add(Shared.Options.FileOption);
                SetAction(result => Execute(
                    result.GetValue(Shared.Options.FileOption)
                ));
            }

            private static async Task Execute(FileInfo? info)
            {
                if (info is not { Exists: true })
                {
                    Console.WriteLine($"File {info?.Name} does not exist.");
                    return;
                }

                var source = await File.ReadAllTextAsync(info.FullName);
                using var tree = SyntaxTree.Parse(source, Console.WriteLine);

                if (!tree.IsHealthy) return;

                var chunk = Compiler.Execute(tree);
                using var vm = new VirtualMachine();
                vm.StdOut += Console.WriteLine;
                vm.Execute(chunk);
            }
        }
    }
}