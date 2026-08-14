namespace Imparsable.Parsing.Interfaces;

public interface ISourceMarker
{
    public int Line { get; }
    public int Column { get; }
}