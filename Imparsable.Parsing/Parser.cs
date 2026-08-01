namespace Imparsable.Parsing;

public abstract partial class Parser<TToken> where TToken : Enum
{
    public interface ISyntax
    {
        public Lexer<TToken>.Token Token { get; }

        public delegate ISyntax Func(Context context);
    }

    public interface IProduction<out TSyntax> where TSyntax : ISyntax
    {
        public static abstract TSyntax Parse(Context context);
    }
}