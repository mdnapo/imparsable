using System.CommandLine;
using Imparsable.CLI.Interfaces;
using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;

namespace Imparsable.CLI.Commands;

internal sealed partial class Imp
{
    internal sealed partial class Calculator
    {
        internal sealed class Disassemble : Command, ISubCommandOf<Calculator>
        {
            public Disassemble() : base("disassemble", "Disassemble calculator file")
            {
                Aliases.Add("d");
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

                if (!diagnostics.IsHealthy || Disassembler.Disassemble(tree, diagnostics) is not { } output) return;

                Console.WriteLine(output);
            }
        }
    }
}