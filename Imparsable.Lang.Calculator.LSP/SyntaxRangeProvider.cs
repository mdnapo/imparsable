using System.Globalization;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.LSP;

public class SyntaxRangeProvider : ISyntaxVisitor<SyntaxRange>
{
    public static readonly SyntaxRangeProvider Instance = new();

    public SyntaxRange Visit(AssignmentExpression node) =>
        node.Value.Accept(this) with
        {
            StartLine = node.Value.Token.Line,
            StartColumn = node.Value.Token.Column
        };

    public SyntaxRange Visit(BinaryExpression node) =>
        node.RightOperand.Accept(this) with
        {
            StartLine = node.LeftOperand.Token.Line,
            StartColumn = node.LeftOperand.Token.Column
        };

    public SyntaxRange Visit(BlockStatement node) => new(
        node.LeftBrace.Line,
        node.LeftBrace.Column,
        node.RightBrace.Line,
        node.RightBrace.Column
    );

    public SyntaxRange Visit(BoolLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + (node.Value ? 4 : 5)
    );

    public SyntaxRange Visit(ConstStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(ElseIfStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public SyntaxRange Visit(ExpressionStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(ForStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public SyntaxRange Visit(GroupingExpression node) => new(
        node.LeftParenthesis.Line,
        node.LeftParenthesis.Column,
        node.RightParenthesis.Line,
        node.RightParenthesis.Column
    );

    public SyntaxRange Visit(IdentifierExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Symbol.Length
    );

    public SyntaxRange Visit(IfStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public SyntaxRange Visit(NumericLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Value.ToString(CultureInfo.InvariantCulture).Length
    );

    public SyntaxRange Visit(PrintStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(StringLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Value.Length
    );

    public SyntaxRange Visit(UnaryExpression node) =>
        node.Operand.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public SyntaxRange Visit(VarStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(WhileStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public SyntaxRange Visit(BreakStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(ContinueStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public SyntaxRange Visit(ErrorNode node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column
    );
}