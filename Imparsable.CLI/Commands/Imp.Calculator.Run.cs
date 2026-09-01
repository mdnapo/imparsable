using System.CommandLine;
using Imparsable.CLI.Interfaces;
using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Virtualization;
using Imparsable.Toolchain;

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
                using var diagnostics = new DiagnosticsProvider();
                diagnostics.Published += Console.WriteLine;
                var tree = SyntaxTree.Parse(source, diagnostics);

                if (!diagnostics.IsHealthy || Compiler.Execute(tree, diagnostics) is not { } chunk) return;

                using var vm = new VirtualMachine();
                vm.StdOut += Console.WriteLine;
                vm.Execute(chunk);

                Console.WriteLine(Disassembler.Disassemble(tree, diagnostics));
            }
        }
    }
}