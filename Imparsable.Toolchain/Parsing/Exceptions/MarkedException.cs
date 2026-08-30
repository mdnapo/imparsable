using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing.Exceptions;

public abstract class MarkedException(ISourceMarker marker, string? message) : Exception(message)
{
    public ISourceMarker Marker { get; } = marker;
}