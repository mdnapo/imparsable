using System.CommandLine;

namespace Imparsable.CLI.Interfaces;

// ReSharper disable once UnusedTypeParameter
internal interface ISubCommandOf<T> where T : Command;
