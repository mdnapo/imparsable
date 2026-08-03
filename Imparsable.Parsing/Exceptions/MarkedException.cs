namespace Imparsable.Parsing.Exceptions;

public abstract class MarkedException<T>(string? message) : Exception(message) where T : Exception
{
    public static T Create(ISourceMarker marker, string? message)
    {
        message = $"{marker.File}\n\t[line: {marker.Line}, column: {marker.Column}] {message}.";

        return Activator.CreateInstance(typeof(T), message) as T ??
               throw new InvalidOperationException($"Could not instantiate {typeof(T).Name}.");
    }

    public static void Throw(ISourceMarker marker, string? message)
    {
        message = $"{marker.File}\n\t[line: {marker.Line}, column: {marker.Column}] {message}.";

        if (Activator.CreateInstance(typeof(T), message) is not T exception)
            throw new InvalidOperationException($"Could not instantiate {typeof(T).Name}.");

        throw exception;
    }
}