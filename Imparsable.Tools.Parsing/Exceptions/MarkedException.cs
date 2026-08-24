using Imparsable.Tools.Parsing.Interfaces;

namespace Imparsable.Tools.Parsing.Exceptions;

public abstract class MarkedException(ISourceMarker marker, string? message) : Exception(message)
{
    public ISourceMarker Marker { get; } = marker;
}