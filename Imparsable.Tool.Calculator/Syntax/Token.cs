using Imparsable.Parsing.Attributes;

namespace Imparsable.Tool.Calculator.Syntax;

public enum Token
{
    [SingleCharacter<Token>('.')]
    DOT = 16,

    [Whitespace<Token>]
    WHITESPACE = 17,

    [NewLine<Token>]
    NEWLINE = 18,

    [Number<Token>]
    NUMBER = 0,

    [SingleQuoteString<Token>]
    [DoubleQuoteString<Token>]
    STRING = 1,

    [Keyword]
    VAR = 2,

    [Keyword]
    CONST = 3,

    [Identifier<Token>]
    IDENTIFIER = 4,

    [SingleCharacter<Token>(';')]
    SEMICOLON = 5,

    [SingleCharacter<Token>('+')]
    PLUS = 6,

    [SingleCharacter<Token>('-')]
    MINUS = 7,

    [SingleCharacter<Token>('*')]
    STAR = 8,

    [SingleCharacter<Token>('/')]
    SLASH = 9,

    [SingleCharacter<Token>('=')]
    EQUALS = 12,

    [SingleCharacter<Token>('(')]
    LEFT_PARENTHESIS = 13,

    [SingleCharacter<Token>(')')]
    RIGHT_PARENTHESIS = 14,

    [Keyword]
    PRINT = 15,

    [Unexpected]
    UNEXPECTED = 10,

    [End]
    END = 11,
}