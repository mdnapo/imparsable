namespace Imparsable.Lang.Calculator.Compilation;

public enum OpCode : byte
{
    GET_LOCAL,
    SET_LOCAL,
    POP,
    
    ADD,
    SUB,
    MUL,
    DIV,
    
    CONCAT,
    
    OR,
    AND,
    EQUAL,
    NOT_EQUAL,
    LOWER_THAN,
    LOWER_EQUAL,
    GREATER_EQUAL,
    GREATER_THAN,
    
    NUM_CONST,
    BOOL_CONST,
    STRING_CONST,
    NEGATE_BOOL,
    NEGATE_NUM,
    
    JMP,
    JMP_FALSE,
    TO_STRING,
    PRINT,
}