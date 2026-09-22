using System.Reflection;

namespace Imparsable.Lang.Calculator;

public static class CalculatorGrammar
{
    public static readonly string Text = ReadResource();

    private static string ReadResource()
    {
        var assembly = typeof(CalculatorGrammar).GetTypeInfo().Assembly;
        var @namespace = assembly.GetName().Name;
        var resource = $"{@namespace}.Calculator.g4";
        using var stream = assembly.GetManifestResourceStream(resource);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
        return reader.ReadToEnd();
    }
}