using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter(SyntaxTree tree) : ISyntaxVisitor
{
    private readonly Document _document = new(tree);
    private readonly Stack<ISyntax> _blockContext = new();

    private void IncrementDepth() => _document.IncrementDepth();
    private void DecrementDepth() => _document.DecrementDepth();
    private void Space() => _document.Space();
    private void NewLine() => _document.NewLine();
    private void Write(Lexer<Token>.Token token) => _document.Write(token);
    private void WriteLine(Lexer<Token>.Token token) => _document.WriteLine(token);

    public static string Format(SyntaxTree tree, int tabSize = 4)
    {
        var formatter = new Formatter(tree);

        foreach (var root in tree.Roots)
            root.Accept(formatter);

        return new Renderer(tree.Source, tabSize).Render(formatter._document);
    }

    private void WriteBody(ISyntax body)
    {
        if (body is BlockStatement)
        {
            Space();
            body.Accept(this);
            return;
        }

        NewLine();
        IncrementDepth();

        body.Accept(this);

        DecrementDepth();
    }

    public void Visit(AssignmentExpression node)
    {
        node.Target.Accept(this);
        Space();
        Write(node.Operator);
        Space();
        node.Value.Accept(this);
    }

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        Space();
        Write(node.Operator);
        Space();
        node.RightOperand.Accept(this);
    }

    public void Visit(BlockStatement node)
    {
        Write(node.LeftBrace);
        NewLine();

        tree.SymbolRoot.Push(node);
        _blockContext.Push(node);

        IncrementDepth();

        foreach (var statement in node.Body)
            statement.Accept(this);

        DecrementDepth();

        _blockContext.Pop();
        tree.SymbolRoot.Pop();

        Write(node.RightBrace);
    }

    public void Visit(BoolLiteralExpression node) => Write(node.Token);

    public void Visit(ConstStatement node)
    {
        Write(node.Keyword);
        Space();
        Write(node.Identifier);
        Space();
        Write(node.Assignment);
        Space();
        node.Initializer.Accept(this);
        WriteLine(node.SemiColon);
    }

    public void Visit(ElseIfStatement node)
    {
        Write(node.ElseKeyword);
        Space();
        Write(node.IfKeyword);
        Space();
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);

        WriteBody(node.Body);

        if (node.Next is { } next)
        {
            Space();
            next.Accept(this);
        }
    }

    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        WriteLine(node.SemiColon);
    }

    public void Visit(ForStatement node)
    {
        Write(node.Keyword);
        Space();
        Write(node.LeftParenthesis);

        tree.SymbolRoot.Push(node);
        _blockContext.Push(node);

        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
        }
        else if (node.InitializerSemiColon is not null)
        {
            Write(node.InitializerSemiColon.Value);
        }

        Space();

        node.Condition.Accept(this);
        Write(node.ConditionSemiColon);
        Space();

        node.Increment?.Accept(this);

        Write(node.RightParenthesis);

        WriteBody(node.Body);

        _blockContext.Pop();
        tree.SymbolRoot.Pop();

        NewLine();
    }

    public void Visit(GroupingExpression node)
    {
        Write(node.LeftParenthesis);
        node.Expression.Accept(this);
        Write(node.RightParenthesis);
    }

    public void Visit(IdentifierExpression node) => Write(node.Token);

    public void Visit(IfStatement node)
    {
        _blockContext.Push(node);

        Write(node.Keyword);
        Space();
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);

        WriteBody(node.Body);

        if (node.ElseIf is { } elseIf)
        {
            Space();
            elseIf.Accept(this);
        }

        if (node.Else is { } @else)
        {
            Space();
            Write(node.ElseKeyword!.Value);

            WriteBody(@else);
        }

        NewLine();

        _blockContext.Pop();
    }

    public void Visit(NumericLiteralExpression node) => Write(node.Token);

    public void Visit(PrintStatement node)
    {
        Write(node.Keyword);
        Space();
        node.Expression.Accept(this);
        WriteLine(node.SemiColon);
    }

    public void Visit(StringLiteralExpression node) => Write(node.Token);

    public void Visit(UnaryExpression node)
    {
        Write(node.Op);
        node.Operand.Accept(this);
    }

    public void Visit(VarStatement node)
    {
        Write(node.Keyword);
        Space();
        Write(node.Identifier);

        if (node.Initializer is not null)
        {
            Space();
            Write(node.Assignment!.Value);
            Space();
            node.Initializer.Accept(this);
        }

        if (_blockContext.Count == 0 || _blockContext.Peek() is not ForStatement)
        {
            WriteLine(node.SemiColon);
        }
        else
        {
            Write(node.SemiColon);
        }
    }

    public void Visit(WhileStatement node)
    {
        _blockContext.Push(node);

        Write(node.Keyword);
        Space();
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);

        WriteBody(node.Body);

        _blockContext.Pop();

        NewLine();
    }

    public void Visit(BreakStatement node)
    {
        Write(node.Keyword);
        WriteLine(node.SemiColon);
    }

    public void Visit(ContinueStatement node)
    {
        Write(node.Keyword);
        WriteLine(node.SemiColon);
    }

    public void Visit(ErrorNode node) => Write(node.Token);
}