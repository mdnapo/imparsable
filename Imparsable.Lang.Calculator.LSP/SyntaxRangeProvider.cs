using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.LSP;

public sealed class SyntaxRangeProvider : ISyntaxVisitor<SyntaxRange>
{
    public static SyntaxRangeProvider Instance { get; } = new();

    public SyntaxRange Visit(ISyntax syntax) => syntax.Accept(this);

    public SyntaxRange Visit(AssignmentExpression node) =>
        From(node.Target.Token, Visit(node.Value));

    public SyntaxRange Visit(BinaryExpression node) =>
        From(node.LeftOperand.Token, Visit(node.RightOperand));

    public SyntaxRange Visit(BlockStatement node) =>
        From(node.LeftBrace, node.RightBrace);

    public SyntaxRange Visit(BoolLiteralExpression node) =>
        From(node.Token);

    public SyntaxRange Visit(BreakStatement node) =>
        From(node.Keyword, node.SemiColon);

    public SyntaxRange Visit(ConstStatement node) =>
        From(node.Keyword, node.SemiColon);

    public SyntaxRange Visit(ContinueStatement node) =>
        From(node.Keyword, node.SemiColon);

    public SyntaxRange Visit(ErrorNode node) =>
        From(node.Token);

    public SyntaxRange Visit(ExpressionStatement node) =>
        From(node.Expression.Token, node.SemiColon);

    public SyntaxRange Visit(GroupingExpression node) =>
        From(node.LeftParenthesis, node.RightParenthesis);

    public SyntaxRange Visit(IdentifierExpression node) =>
        From(node.Token);

    public SyntaxRange Visit(NumericLiteralExpression node) =>
        From(node.Token);

    public SyntaxRange Visit(PrintStatement node) =>
        From(node.Keyword, node.SemiColon);

    public SyntaxRange Visit(StringLiteralExpression node) =>
        From(node.Token);

    public SyntaxRange Visit(UnaryExpression node) =>
        From(node.Op, Visit(node.Operand));

    public SyntaxRange Visit(VarStatement node) =>
        From(node.Keyword, node.SemiColon);

    public SyntaxRange Visit(WhileStatement node) =>
        From(node.Keyword, Visit(node.Body));

    public SyntaxRange Visit(ForStatement node) =>
        From(node.Keyword, Visit(node.Body));

    public SyntaxRange Visit(ElseIfStatement node)
    {
        if (node.Next is not null)
            return From(node.Token, Visit(node.Next));

        return From(node.Token, Visit(node.Body));
    }

    public SyntaxRange Visit(IfStatement node)
    {
        if (node.Else is not null)
            return From(node.Keyword, Visit(node.Else));

        if (node.ElseIf is not null)
            return From(node.Keyword, Visit(node.ElseIf));

        return From(node.Keyword, Visit(node.Body));
    }

    private static SyntaxRange From(Lexer<Token>.Token token) =>
        SyntaxRange.From(token);

    private static SyntaxRange From(Lexer<Token>.Token start, Lexer<Token>.Token end) =>
        SyntaxRange.From(start, end);

    private static SyntaxRange From(Lexer<Token>.Token start, SyntaxRange end) =>
        SyntaxRange.From(start, end);
}