using Imparsable.Parsing;
using Imparsable.Tool.Calculator;
using Imparsable.Tool.Calculator.Syntax;

const string file = "test/program.clc";
var source = await File.ReadAllTextAsync(file);

var runtime = new Runtime();
runtime.StdOut += Console.WriteLine;
runtime.Execute(source);

var rules = Lexer<Token>.GetRules();
var keywords = ParserConfiguration<Token>.GetKeywords();

Console.WriteLine();