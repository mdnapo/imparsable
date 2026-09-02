using Bootsharp;
using Bootsharp.Inject;
using Microsoft.Extensions.DependencyInjection;

[assembly: Export(typeof(Calculator))]

new ServiceCollection()
    .AddBootsharp()
    .AddSingleton<Calculator>()
    .BuildServiceProvider()
    .RunBootsharp();
