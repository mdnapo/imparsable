using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing.Interfaces;

public interface ISymbol
{
    Lexer<Token>.Token Symbol { get; }
}