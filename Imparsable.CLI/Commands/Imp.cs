using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using Imparsable.CLI.Extensions;
using Imparsable.CLI.Interfaces;

namespace Imparsable.CLI.Commands;

internal sealed partial class Imp : RootCommand
{
    public Imp(IEnumerable<ISubCommandOf<Imp>> commands) : base(description: "The Imparsable command line tools")
    {
        this.RegisterSubCommands(commands);
        
        

        foreach (var option in Options)
        {
            if (option is not HelpOption defaultHelpOption) continue;
            defaultHelpOption.Action = new CustomHelpAction((HelpAction)defaultHelpOption.Action!);
            break;
        }
    }

    private class CustomHelpAction(HelpAction action) : SynchronousCommandLineAction
    {
        public override int Invoke(ParseResult parseResult)
        {
            Console.WriteLine("""
                               ____  _              ____ _     ___ 
                              / ___|| | _____  __  / ___| |   |_ _|
                              \___ \| |/ _ \ \/ / | |   | |    | | 
                               ___) | | (_) >  <  | |___| |___ | | 
                              |____/|_|\___/_/\_\  \____|_____|___|
                               
                              """);

            var result = action.Invoke(parseResult);

            return result;
        }
    }
}