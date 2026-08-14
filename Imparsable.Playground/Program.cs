using Imparsable.Tool.Calculator;

const string file = "test/program.clc";
var source = await File.ReadAllTextAsync(file);
var runtime = new Runtime();
runtime.StdOut += Console.WriteLine;
runtime.Execute(source);

Console.WriteLine();