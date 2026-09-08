grammar Calculator;

program
    : statement* EOF
    ;

statement
    : constStatement
    | varStatement
    | printStatement
    | blockStatement
    | forStatement
    | whileStatement
    | breakStatement
    | continueStatement
    | ifStatement
    | expressionStatement
    ;

constStatement
    : CONST IDENTIFIER EQUAL expression SEMICOLON
    ;

varStatement
    : VAR IDENTIFIER (EQUAL expression)? SEMICOLON
    ;

printStatement
    : PRINT expression SEMICOLON
    ;

blockStatement
    : LEFT_BRACE statement* RIGHT_BRACE
    ;

forStatement
    : FOR LEFT_PARENTHESIS forInitializer expression? SEMICOLON expression? RIGHT_PARENTHESIS statement
    ;

forInitializer
    : SEMICOLON
    | varStatement
    | expressionStatement
    ;

whileStatement
    : WHILE LEFT_PARENTHESIS expression RIGHT_PARENTHESIS statement
    ;

breakStatement
    : BREAK SEMICOLON
    ;

continueStatement
    : CONTINUE SEMICOLON
    ;

ifStatement
    : IF LEFT_PARENTHESIS expression RIGHT_PARENTHESIS statement
      (ELSE IF LEFT_PARENTHESIS expression RIGHT_PARENTHESIS statement)*
      (ELSE statement)?
    ;

expressionStatement
    : expression SEMICOLON
    ;

expression
    : assignmentExpression
    ;

assignmentExpression
    : binaryExpression (assignmentOperator assignmentExpression)?
    ;

assignmentOperator
    : EQUAL
    | PLUS_EQUAL
    | MINUS_EQUAL
    | STAR_EQUAL
    | SLASH_EQUAL
    ;

binaryExpression
    : logicalOrExpression
    ;

logicalOrExpression
    : logicalAndExpression (OR_OR logicalAndExpression)*
    ;

logicalAndExpression
    : equalityExpression (AND_AND equalityExpression)*
    ;

equalityExpression
    : moduloExpression ((BANG_EQUAL | EQUAL_EQUAL | LOWER_THAN | LOWER_EQUAL | GREATER_THAN | GREATER_EQUAL) moduloExpression)*
    ;

moduloExpression
    : additiveExpression (MODULO additiveExpression)*
    ;

additiveExpression
    : multiplicativeExpression ((PLUS | MINUS) multiplicativeExpression)*
    ;

multiplicativeExpression
    : unaryExpression ((STAR | SLASH) unaryExpression)*
    ;

unaryExpression
    : (BANG | MINUS) unaryExpression
    | primaryExpression
    ;

primaryExpression
    : IDENTIFIER
    | LEFT_PARENTHESIS expression RIGHT_PARENTHESIS
    | STRING
    | TRUE
    | FALSE
    | NUMBER
    ;

// Lexer rules

CONST       : 'const';
VAR         : 'var';
PRINT       : 'print';
IF          : 'if';
ELSE        : 'else';
TRUE        : 'true';
FALSE       : 'false';
FOR         : 'for';
WHILE       : 'while';
BREAK       : 'break';
CONTINUE    : 'continue';

BANG_EQUAL     : '!=';
EQUAL_EQUAL    : '==';
GREATER_EQUAL  : '>=';
LOWER_EQUAL    : '<=';
OR_OR          : '||';
AND_AND        : '&&';
PLUS_EQUAL     : '+=';
MINUS_EQUAL    : '-=';
STAR_EQUAL     : '*=';
SLASH_EQUAL    : '/=';

SEMICOLON          : ';';
BANG               : '!';
GREATER_THAN       : '>';
LOWER_THAN         : '<';
MODULO             : '%';
PLUS               : '+';
MINUS              : '-';
STAR               : '*';
SLASH              : '/';
EQUAL               : '=';
LEFT_PARENTHESIS    : '(';
RIGHT_PARENTHESIS   : ')';
LEFT_BRACE          : '{';
RIGHT_BRACE         : '}';

NUMBER
    : DIGIT+ ('.' DIGIT+)?
    ;

STRING
    : '"' (~["])* '"'
    | '\'' (~['])* '\''
    ;

IDENTIFIER
    : [a-zA-Z_] [a-zA-Z0-9_]*
    ;
    
WHITESPACE
    : [ \t]+ -> skip
    ;

    
NEWLINE
    : '\r'? '\n' -> skip
    ;

fragment DIGIT
    : [0-9]
    ;

UNEXPECTED
    : .
    ;
