using System.Reflection;

namespace Imparsable.Lang.Calculator;

public static class CalculatorGrammar
{
    public static readonly string Text = ReadResource();

    // Source - https://stackoverflow.com/a/3314213
    // Posted by dtb, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-09-08, License - CC BY-SA 4.0
    private static string ReadResource()
    {
        var assembly = typeof(CalculatorGrammar).GetTypeInfo().Assembly;
        var @namespace = typeof(CalculatorGrammar).Namespace;
        var resource = $"{@namespace}.Calculator.g4";
        using var stream = assembly.GetManifestResourceStream(resource);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
        return reader.ReadToEnd();
    }
}