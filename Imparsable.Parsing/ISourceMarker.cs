namespace Imparsable.Parsing;

public interface ISourceMarker
{
    public int Line { get; }
    public int Column { get; }
}