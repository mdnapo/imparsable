namespace Imparsable.Parsing;

public interface ISourceMarker
{
    public string File { get; }
    public int Line { get; }
    public int Column { get; }
}