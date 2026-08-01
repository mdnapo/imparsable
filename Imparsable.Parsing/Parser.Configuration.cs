namespace Imparsable.Parsing;

public abstract partial class Parser<TToken>
{
    public class Configuration
    {
        public Mode Mode { get; set; } = Mode.Strict;
        public int SpaceSize { get; set; } = 1;
        public int TabSize { get; set; } = 4;
        
        public required TToken Identifier { get; init; }
        public required TToken Unexpected { get; init; }
        public required TToken End { get; init; }
    }
}