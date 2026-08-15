using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Syntax;

var tree = SyntaxTree.Parse(await File.ReadAllTextAsync("test/program.clc"));
using var runtime = new Runtime();
runtime.StdOut += Console.WriteLine;
runtime.Execute(tree);

Console.WriteLine();