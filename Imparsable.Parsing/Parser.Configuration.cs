namespace Imparsable.Parsing;

public static partial class Parser<TToken>
{
    public class Configuration
    {
        public int SpaceSize { get; set; } = 1;
        public int TabSize { get; set; } = 4;
        
        public required TToken Identifier { get; init; }
        public required TToken Unexpected { get; init; }
        public required TToken End { get; init; }
    }
}