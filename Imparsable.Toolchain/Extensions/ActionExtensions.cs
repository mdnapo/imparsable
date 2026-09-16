namespace Imparsable.Toolchain.Extensions;

public static class ActionExtensions
{
    public static void Clear(this Action? action)
    {
        if (action is null) return;

        foreach (var @delegate in action.GetInvocationList())
            action -= @delegate as Action;
    }
    
    public static void Clear<T>(this Action<T>? action)
    {
        if (action is null) return;

        foreach (var @delegate in action.GetInvocationList())
            action -= @delegate as Action<T>;
    }
}