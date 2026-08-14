using Imparsable.Parsing.Attributes;

namespace Imparsable.Parsing.Extensions;

internal static class EnumExtensions
{
    internal static bool IsIdentifier<TToken>(this Enum @enum) where TToken : Enum =>
        @enum
            .GetType()
            .GetField(@enum.ToString())!
            .IsDefined(typeof(IdentifierAttribute<TToken>), inherit: false);
}