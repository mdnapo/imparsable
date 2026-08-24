namespace Imparsable.Tools.Parsing.Interfaces;

public interface ISourceMarker
{
    public int Offset { get; }
    public int Length { get; }
    public int Line { get; }
    public int Column { get; }
}