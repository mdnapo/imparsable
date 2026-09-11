// namespace Imparsable.Toolchain.Parsing.Attributes;
//
// public abstract class TriviaRuleAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
// {
//
//     protected void HandleIgnore(
//         bool ignore,
//         TToken type,
//         Lexer<TToken>.Context context,
//         Source src,
//         int line,
//         int column
//     )
//     {
//         if (ignore)
//         {
//             src.Ignore();
//         }
//         else
//         {
//             var range = src.Extract();
//             context.AddToken(type, range.Offset, range.Length, line, column);
//         }
//     }
// }