using System.CommandLine;

namespace Imparsable.CLI;

public static class Shared
{
    public static class Options
    {
        public static readonly Option<FileInfo> FileOption = new("file", aliases: ["-f", "--file"]);
    }
}