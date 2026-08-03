using System.Reflection;
using Imparsable.Parsing.Attributes;

namespace Imparsable.Parsing;

public static partial class Parser<TToken> where TToken : Enum
{
    public class Configuration(IEnumerable<TToken> keywords)
    {
        public int TabSize => 4;

        public TToken Identifier { get; } = GetValue<IdentifierAttribute>();
        public TToken Unexpected { get; } = GetValue<UnexpectedAttribute>();
        public TToken End { get; } = GetValue<EndAttribute>();

        public List<Lexer<TToken>.Keyword> Keywords { get; } =
            [.. keywords.Select(type => new Lexer<TToken>.Keyword { Name = type.ToString().ToLower(), Type = type })];

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
    }
}