grammar Calculator;

// --------------------
// Parser rules
// --------------------

program
    : statement* EOF
    ;

statement
    : constStatement
    | varStatement
    | printStatement
    | ifStatement
    | whileStatement
    | forStatement
    | block
    | expressionStatement
    ;

constStatement
    : CONST IDENTIFIER ASSIGN expression SEMICOLON
    ;

varStatement
    : VAR IDENTIFIER (ASSIGN expression)? SEMICOLON
    ;

printStatement
    : PRINT expression SEMICOLON
    ;

expressionStatement
    : expression SEMICOLON
    ;

block
    : LBRACE statement* RBRACE
    ;

ifStatement
    : IF LPAREN expression RPAREN statement
      (ELSE IF LPAREN expression RPAREN statement)*
      (ELSE statement)?
    ;

whileStatement
    : WHILE LPAREN expression RPAREN statement
    ;

forStatement
    : FOR LPAREN
      forInitializer? SEMICOLON
      expression? SEMICOLON
      expression?
      RPAREN
      statement
    ;

forInitializer
    : varInitializer
    | expression
    ;

varInitializer
    : VAR IDENTIFIER (ASSIGN expression)?
    ;

// --------------------
// Expressions
// --------------------

expression
    : assignmentExpression
    ;

assignmentExpression
    : IDENTIFIER assignmentOperator assignmentExpression
    | binaryExpression
    ;

assignmentOperator
    : ASSIGN
    | PLUS_ASSIGN
    | MINUS_ASSIGN
    | STAR_ASSIGN
    | SLASH_ASSIGN
    ;

binaryExpression
    : binaryExpression (OR_OR) binaryExpression
    | binaryExpression (AND_AND) binaryExpression
    | binaryExpression (EQUAL_EQUAL | BANG_EQUAL |LESS | LESS_EQUAL | GREATER | GREATER_EQUAL) binaryExpression
    | binaryExpression (PLUS | MINUS) binaryExpression
    | binaryExpression (STAR | SLASH) binaryExpression
    | unaryExpression
    ;

unaryExpression
    : (BANG | MINUS) unaryExpression
    | primaryExpression
    ;

primaryExpression
    : NUMBER
    | STRING
    | TRUE
    | FALSE
    | IDENTIFIER
    | LPAREN expression RPAREN
    ;

// --------------------
// Lexer rules
// --------------------

CONST
    : 'const'
    ;

VAR
    : 'var'
    ;

PRINT
    : 'print'
    ;

IF
    : 'if'
    ;

ELSE
    : 'else'
    ;

WHILE
    : 'while'
    ;

FOR
    : 'for'
    ;

TRUE
    : 'true'
    ;

FALSE
    : 'false'
    ;

PLUS_ASSIGN
    : '+='
    ;

MINUS_ASSIGN
    : '-='
    ;

STAR_ASSIGN
    : '*='
    ;

SLASH_ASSIGN
    : '/='
    ;

EQUAL_EQUAL
    : '=='
    ;

BANG_EQUAL
    : '!='
    ;

LESS_EQUAL
    : '<='
    ;
    
AND_AND
    : '&&'
    ;
    
OR_OR
    : '||'
    ;

GREATER_EQUAL
    : '>='
    ;

ASSIGN
    : '='
    ;

PLUS
    : '+'
    ;

MINUS
    : '-'
    ;

STAR
    : '*'
    ;

SLASH
    : '/'
    ;

BANG
    : '!'
    ;

LESS
    : '<'
    ;

GREATER
    : '>'
    ;

LPAREN
    : '('
    ;

RPAREN
    : ')'
    ;

LBRACE
    : '{'
    ;

RBRACE
    : '}'
    ;

SEMICOLON
    : ';'
    ;

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

fragment DIGIT
    : [0-9]
    ;

WHITESPACE
    : [ \t\r\n]+ -> skip
    ;
