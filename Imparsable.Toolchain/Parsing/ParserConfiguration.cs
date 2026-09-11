using System.Reflection;
using Imparsable.Toolchain.Parsing.Attributes;

namespace Imparsable.Toolchain.Parsing;

public class ParserConfiguration<TToken> where TToken : Enum
{
    private readonly IReadOnlyList<Keyword<TToken>> _keywords = GetKeywords();

    public static readonly ParserConfiguration<TToken> Default = new();

    public readonly TToken[] TriviaTokens =
    [
        .. new List<TToken?>([
                GetToken<WhitespaceAttribute<TToken>>(),
                GetToken<NewLineAttribute<TToken>>(),
                .. GetTokens<SingleLineCommentAttribute<TToken>>(),
                .. GetTokens<MultiLineCommentAttribute<TToken>>()
            ])
            .Where(x => x is not null)
            .Cast<TToken>()
    ];

    public TToken Unexpected { get; } = GetToken<UnexpectedAttribute>();
    public TToken Error { get; } = GetToken<ErrorAttribute>();
    public TToken End { get; } = GetToken<EndAttribute>();
    public int TabSize => 4;

    private static TToken GetToken<TAttribute>() where TAttribute : Attribute
    {
        var fields = typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsDefined(typeof(TAttribute), inherit: false))
            .ToArray();

        return fields.Length switch
        {
            0 => throw new InvalidOperationException($"Enum '{typeof(TToken).FullName}' has no member tagged with [{typeof(TAttribute).Name}]."),
            1 => (TToken)fields.First().GetValue(null)!,
            _ => throw new InvalidOperationException($"Enum '{typeof(TToken).FullName}' has multiple members tagged with [{typeof(TAttribute).Name}].")
        };
    }

    // ReSharper disable once ReturnTypeCanBeNotNullable
    private static IEnumerable<TToken> GetTokens<TAttribute>() where TAttribute : Attribute
    {
        var fields = typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsDefined(typeof(TAttribute), inherit: false))
            .ToArray();

        return fields.Select(field => (TToken)field.GetValue(null)!);
    }

    public Keyword<TToken>? IsKeyword(string text) =>
        _keywords.FirstOrDefault(keyword => keyword.Name.Equals(text));

    private static IReadOnlyList<Keyword<TToken>> GetKeywords() =>
    [
        .. typeof(TToken)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(field => field
                .GetCustomAttributes(inherit: false)
                .OfType<KeywordAttribute>()
                .Select(_ => new Keyword<TToken>((TToken)field.GetValue(null)!)))
    ];
}