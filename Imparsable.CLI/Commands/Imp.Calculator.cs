using System.CommandLine;
using Imparsable.CLI.Extensions;
using Imparsable.CLI.Interfaces;

namespace Imparsable.CLI.Commands;

internal sealed partial class Imp
{
    internal sealed partial class Calculator : Command, ISubCommandOf<Imp>
    {
        public Calculator(IEnumerable<ISubCommandOf<Calculator>> commands) :
            base("clc", "The Imparsable calculator commands")
        {
            this.RegisterSubCommands(commands);
        }
    }
}