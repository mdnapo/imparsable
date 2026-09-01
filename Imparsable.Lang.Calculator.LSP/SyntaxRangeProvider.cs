using System.Globalization;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.LSP;

public class SyntaxRangeProvider : ISyntaxVisitor<CompletionRange>
{
    public static readonly SyntaxRangeProvider Instance = new();

    public CompletionRange Visit(AssignmentExpression node) =>
        node.Value.Accept(this) with
        {
            StartLine = node.Value.Token.Line,
            StartColumn = node.Value.Token.Column
        };

    public CompletionRange Visit(BinaryExpression node) =>
        node.RightOperand.Accept(this) with
        {
            StartLine = node.LeftOperand.Token.Line,
            StartColumn = node.LeftOperand.Token.Column
        };

    public CompletionRange Visit(BlockStatement node) => new(
        node.LeftBrace.Line,
        node.LeftBrace.Column,
        node.RightBrace.Line,
        node.RightBrace.Column
    );

    public CompletionRange Visit(BoolLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + (node.Value ? 4 : 5)
    );

    public CompletionRange Visit(ConstStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(ElseIfStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public CompletionRange Visit(ExpressionStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(ForStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public CompletionRange Visit(GroupingExpression node) => new(
        node.LeftParenthesis.Line,
        node.LeftParenthesis.Column,
        node.RightParenthesis.Line,
        node.RightParenthesis.Column
    );

    public CompletionRange Visit(IdentifierExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Symbol.Length
    );

    public CompletionRange Visit(IfStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public CompletionRange Visit(NumericLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Value.ToString(CultureInfo.InvariantCulture).Length
    );

    public CompletionRange Visit(PrintStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(StringLiteralExpression node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column + node.Value.Length
    );

    public CompletionRange Visit(UnaryExpression node) =>
        node.Operand.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public CompletionRange Visit(VarStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(WhileStatement node) =>
        node.Body.Accept(this) with
        {
            StartLine = node.Token.Line,
            StartColumn = node.Token.Column
        };

    public CompletionRange Visit(BreakStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(ContinueStatement node) => new(
        node.Token.Line,
        node.Token.Column,
        node.SemiColon.Line,
        node.SemiColon.Column
    );

    public CompletionRange Visit(ErrorNode node) => new(
        node.Token.Line,
        node.Token.Column,
        node.Token.Line,
        node.Token.Column
    );
}