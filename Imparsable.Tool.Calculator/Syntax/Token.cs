using Imparsable.Parsing.Attributes;

namespace Imparsable.Tool.Calculator.Syntax;

public enum Token
{
    [Number<Token>]
    NUMBER = 0,

    [SingleQuoteString<Token>]
    [DoubleQuoteString<Token>]
    STRING = 1,

    VAR = 2,

    CONST = 3,

    [Identifier]
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

    [Unexpected]
    UNEXPECTED = 10,

    [End]
    END = 11,

    [SingleCharacter<Token>('=')]
    EQUALS = 12,

    [SingleCharacter<Token>('(')]
    LEFT_PARENTHESIS = 13,

    [SingleCharacter<Token>(')')]
    RIGHT_PARENTHESIS = 14,

    PRINT = 15,

    [SingleCharacter<Token>('.')]
    DOT = 16,
}