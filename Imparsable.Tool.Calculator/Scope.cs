namespace Imparsable.Tool.Calculator;

public class Scope
{
    private Scope? Parent { get; init; }
    private Dictionary<string, object> Variables { get; } = [];

    public object this[string key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    public Scope CreateChild() => new() { Parent = this };

    public void Declare(string key, object value) => Variables[key] = value;

    private object GetValue(string key)
    {
        if (Variables.TryGetValue(key, out var value))
            return value;

        if (Parent?.Variables.ContainsKey(key) ?? false)
            return Parent.Variables[key];

        throw new KeyNotFoundException(key);
    }

    private void SetValue(string key, object value)
    {
        if (Variables.ContainsKey(key))
            Variables[key] = value;

        if (Parent?.Variables.ContainsKey(key) ?? false)
            Parent.Variables[key] = value;

        throw new KeyNotFoundException(key);
    }
}