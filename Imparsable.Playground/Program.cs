using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Extensions;
using Imparsable.Tool.Calculator.Syntax;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddCalculatorParser()
    .BuildServiceProvider();

await using var scope = services.CreateAsyncScope();
const string file = "test/program.clc";
var source = await File.ReadAllTextAsync(file);

scope.ServiceProvider
    .GetRequiredService<Lexer<Token>.ContextProvider>()
    .Initialize(file, source);

var syntax = scope.ServiceProvider
    .GetRequiredService<Parser<Token, ISyntax>>()
    .Execute<Statement.Production, Statement.Synchronizer>();

var tokens = scope.ServiceProvider
    .GetRequiredService<List<Lexer<Token>.Token>>();

var diagnostics = scope.ServiceProvider
    .GetRequiredService<DiagnosticsCollector>()
    .Diagnostics;

foreach (var diagnostic in diagnostics)
    Console.WriteLine(diagnostic.Report);

Console.WriteLine();