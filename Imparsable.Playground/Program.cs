using Imparsable.Parsing;
using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddCalculatorParser()
    .BuildServiceProvider();

await using var scope = services.CreateAsyncScope();
var file = "test/program.clc";
var source = await File.ReadAllTextAsync(file);

scope.ServiceProvider
    .GetRequiredService<SourceProvider>()
    .Initialize(file, source);

var tokens = scope.ServiceProvider
    .GetRequiredService<Lexer<Token>>()
    .Execute();

Console.WriteLine();