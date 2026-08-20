namespace Imparsable.Tool.Calculator.Extensions;

internal static class ObjectExtensions
{
    internal static T As<T>(this object obj) where T : class => (obj as T)!;
}