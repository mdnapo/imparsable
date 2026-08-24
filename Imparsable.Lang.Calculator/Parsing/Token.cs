using Imparsable.Tools.Parsing.Attributes;

namespace Imparsable.Lang.Calculator.Parsing;

public enum Token
{
    [Whitespace<Token>]
    WHITESPACE,

    [NewLine<Token>]
    NEWLINE,

    [Keyword]
    CONST,

    [Keyword]
    VAR,

    [Keyword]
    PRINT,

    [Keyword]
    IF,

    [Keyword]
    ELSE,

    [Keyword]
    TRUE,

    [Keyword]
    FALSE,

    [Keyword]
    FOR,

    [Keyword]
    WHILE,

    [Number<Token>]
    NUMBER,

    [SingleQuoteString<Token>]
    [DoubleQuoteString<Token>]
    STRING,

    [Identifier<Token>]
    IDENTIFIER,

    [Character<Token>(';')]
    SEMICOLON,

    [Character<Token>('.')]
    DOT,

    [Character<Token>('!')]
    BANG,

    [Character<Token>('>')]
    GREATER_THAN,

    [Character<Token>('<')]
    LOWER_THAN,

    [Sequence<Token>("!=")]
    BANG_EQUAL,

    [Sequence<Token>("==")]
    EQUAL_EQUAL,

    [Sequence<Token>(">=")]
    GREATER_EQUAL,

    [Sequence<Token>("<=")]
    LOWER_EQUAL,

    [Character<Token>('+')]
    PLUS,

    [Sequence<Token>("+=")]
    PLUS_EQUAL,

    [Character<Token>('-')]
    MINUS,

    [Sequence<Token>("-=")]
    MINUS_EQUAL,

    [Character<Token>('*')]
    STAR,

    [Sequence<Token>("*=")]
    STAR_EQUAL,

    [Character<Token>('/')]
    SLASH,

    [Sequence<Token>("/=")]
    SLASH_EQUAL,

    [Character<Token>('=')]
    EQUAL,

    [Character<Token>('(')]
    LEFT_PARENTHESIS,

    [Character<Token>(')')]
    RIGHT_PARENTHESIS,

    [Character<Token>('{')]
    LEFT_BRACE,

    [Character<Token>('}')]
    RIGHT_BRACE,

    [Unexpected]
    UNEXPECTED,

    [End]
    END,
}