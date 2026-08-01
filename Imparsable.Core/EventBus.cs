namespace Imparsable.Core;

public class EventBus
{
    private readonly Dictionary<Type, List<Action<object>>> _handlers = [];

    public void Register<T>(Action<object> handler)
    {
        if (!_handlers.ContainsKey(typeof(T)))
        {
            _handlers[typeof(T)] = [];
        }

        _handlers[typeof(T)].Add(handler);
    }

    public void Emit<T>(T @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        _handlers[typeof(T)].ForEach(handler => handler(@event));
    }
}