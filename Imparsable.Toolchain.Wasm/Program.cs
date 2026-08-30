using Bootsharp;
using Bootsharp.Inject;
using Microsoft.Extensions.DependencyInjection;

[assembly: Export(typeof(CalculatorVM))]

new ServiceCollection()
    .AddBootsharp()
    .AddSingleton<CalculatorVM>()
    .BuildServiceProvider()
    .RunBootsharp();
