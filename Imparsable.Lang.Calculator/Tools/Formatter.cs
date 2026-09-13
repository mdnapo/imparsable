using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter(SyntaxTree tree) : ISyntaxVisitor
{
    private readonly Writer _writer = new(tree);
    private readonly Stack<ISyntax> _blockContext = new();

    private int Depth
    {
        get => _writer.Depth;
        set => _writer.Depth = value;
    }

    private void Space() => _writer.Space();
    private void Indent() => _writer.Indent();
    private void NewLine() => _writer.NewLine();
    private void Write(Lexer<Token>.Token token) => _writer.Write(token);
    private void WriteLine(Lexer<Token>.Token token) => _writer.WriteLine(token);

    public static string Format(SyntaxTree tree)
    {
        var formatter = new Formatter(tree);

        foreach (var root in tree.Roots)
            root.Accept(formatter);

        return formatter._writer.Finish();
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
        Depth++;

        if (_blockContext.Count > 0 && _blockContext.Peek() is BlockStatement)
        {
            Indent();
        }

        WriteLine(node.LeftBrace);
        tree.SymbolRoot.Push(node);
        _blockContext.Push(node);

        foreach (var statement in node.Body)
            statement.Accept(this);

        _blockContext.Pop();
        tree.SymbolRoot.Pop();

        Depth--;
        Indent();

        Write(node.RightBrace);
    }

    public void Visit(BoolLiteralExpression node) => Write(node.Token);

    public void Visit(ConstStatement node)
    {
        Indent();
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
        Space();
        node.Body.Accept(this);

        if (node.Next is { } next)
        {
            Space();
            next.Accept(this);
        }
    }

    public void Visit(ExpressionStatement node)
    {
        Indent();
        node.Expression.Accept(this);
        WriteLine(node.SemiColon);
    }

    public void Visit(ForStatement node)
    {
        Indent();
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
        Space();
        node.Body.Accept(this);

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

        Indent();
        Write(node.Keyword);
        Space();
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);
        Space();
        node.Body.Accept(this);

        if (node.ElseIf is { } elseIf)
        {
            Space();
            elseIf.Accept(this);
        }

        if (node.Else is { } @else)
        {
            Space();
            Write(node.ElseKeyword!.Value);
            Space();
            @else.Accept(this);
        }

        NewLine();

        _blockContext.Pop();
    }

    public void Visit(NumericLiteralExpression node) => Write(node.Token);

    public void Visit(PrintStatement node)
    {
        Indent();
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
        if (_blockContext.Count > 0 && _blockContext.Peek() is not ForStatement)
        {
            Indent();
        }

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

        Indent();
        Write(node.Keyword);
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);
        node.Body.Accept(this);

        _blockContext.Pop();

        NewLine();
    }

    public void Visit(BreakStatement node)
    {
        Indent();
        Write(node.Keyword);
        WriteLine(node.SemiColon);
    }

    public void Visit(ContinueStatement node)
    {
        Indent();
        Write(node.Keyword);
        WriteLine(node.SemiColon);
    }

    public void Visit(ErrorNode node) => Write(node.Token);
}