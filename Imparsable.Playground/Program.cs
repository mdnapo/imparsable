using Imparsable.Parsing;
using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Extensions;
using Imparsable.Tool.Calculator.Syntax;
using Microsoft.Extensions.DependencyInjection;

// var services = new ServiceCollection()
//     .AddCalculator()
//     .BuildServiceProvider();
//
// using var scope = services.CreateScope();
// const string file = "test/program.clc";
// var source = await File.ReadAllTextAsync(file);
//
// var runtime = scope.ServiceProvider.GetRequiredService<Runtime>();
// runtime.StdOut += Console.WriteLine;
// runtime.Execute(file, source);

var rules = Lexer<Token>.GetRules();

Console.WriteLine();