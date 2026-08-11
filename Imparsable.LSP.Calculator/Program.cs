using Imparsable.LSP.Calculator;
using Imparsable.Tool.Calculator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;

var server = await LanguageServer.From(options =>
    options
        .WithInput(Console.OpenStandardInput())
        .WithOutput(Console.OpenStandardOutput())
        .WithLoggerFactory(new LoggerFactory())
        .AddDefaultLoggingProvider()
        .WithServices(services =>
            services
                .AddCalculator()
                .AddSingleton<SourceBuffer>()
        )
        .AddTextDocumentIdentifier<TextDocumentIdentifier>()
        .WithHandler<DidChangeTextDocumentHandler>()
        .WithHandler<DidCloseTextDocumentHandler>()
        .WithHandler<DidOpenTextDocumentHandler>()
        .WithHandler<DidSaveTextDocumentHandler>()
);

await server.WaitForExit;