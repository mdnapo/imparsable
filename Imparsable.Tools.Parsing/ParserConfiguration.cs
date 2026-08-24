using System.Reflection;
using Imparsable.Tools.Parsing.Attributes;

namespace Imparsable.Tools.Parsing;

public class ParserConfiguration<TToken> where TToken : Enum
{
    public static readonly IReadOnlyList<Keyword<TToken>> Keywords = GetKeywords();
    public static readonly ParserConfiguration<TToken> Default = new();

    public int TabSize => 4;

    public TToken Unexpected { get; } = GetValue<UnexpectedAttribute>();
    public TToken End { get; } = GetValue<EndAttribute>();


    private static TToken GetValue<TAttribute>() where TAttribute : Attribute
    {
        var fields = typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsDefined(typeof(TAttribute), inherit: false))
            .ToArray();

        return fields.Length switch
        {
            1 => (TToken)fields[0].GetValue(null)!,

            0 => throw new InvalidOperationException(
                $"Enum '{typeof(TToken).FullName}' has no member tagged with [{typeof(TAttribute).Name}]."),

            _ => throw new InvalidOperationException(
                $"Enum '{typeof(TToken).FullName}' has multiple members tagged with [{typeof(TAttribute).Name}].")
        };
    }

    public static Keyword<TToken>? IsKeyword(string text) =>
        Keywords.FirstOrDefault(keyword => keyword.Name.Equals(text));

    public static IReadOnlyList<Keyword<TToken>> GetKeywords() =>
    [
        .. typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(field => field
                .GetCustomAttributes(inherit: false)
                .OfType<KeywordAttribute>()
                .Select(_ => new Keyword<TToken>((TToken)field.GetValue(null)!)))
    ];
}