namespace Imparsable.Toolchain.Parsing;

public record Keyword<TToken>(TToken Type) where TToken : Enum
{
    public string Name { get; } = Type.ToString().ToLower();
}