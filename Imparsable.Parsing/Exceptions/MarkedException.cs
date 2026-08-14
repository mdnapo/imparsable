using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing.Exceptions;

public abstract class MarkedException(ISourceMarker marker, string? message) : Exception(message)
{
    public ISourceMarker Marker { get; } = marker;
}