using Imparsable.Parsing.Attributes;

namespace Imparsable.Tool.Calculator.Syntax;

public enum Token
{
    NUMBER = 0,
    STRING = 1,
    VAR = 2,
    CONST = 3,
    [Identifier] IDENTIFIER = 4,
    SEMICOLON = 5,
    PLUS = 6,
    MINUS = 7,
    STAR = 8,
    SLASH = 9,
    [Unexpected] UNEXPECTED = 10,
    [End] END = 11,
    EQUALS = 12,
    LEFT_PARENTHESIS = 13,
    RIGHT_PARENTHESIS = 14,
    PRINT = 15,
}