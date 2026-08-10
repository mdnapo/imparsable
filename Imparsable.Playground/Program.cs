using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddCalculator()
    .BuildServiceProvider();

using var scope = services.CreateScope();
const string file = "test/program.clc";
var source = await File.ReadAllTextAsync(file);

var runtime = scope.ServiceProvider.GetRequiredService<Runtime>();
runtime.StdOut += Console.WriteLine;
runtime.Execute(file, source);

Console.WriteLine();