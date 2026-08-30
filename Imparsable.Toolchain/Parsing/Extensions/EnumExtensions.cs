using Imparsable.Toolchain.Parsing.Attributes;

namespace Imparsable.Toolchain.Parsing.Extensions;

internal static class EnumExtensions
{
    internal static bool IsIdentifier<TToken>(this Enum @enum) where TToken : Enum =>
        @enum
            .GetType()
            .GetField(@enum.ToString())!
            .IsDefined(typeof(IdentifierAttribute<TToken>), inherit: false);
}