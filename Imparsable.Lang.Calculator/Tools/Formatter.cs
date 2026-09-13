using System.Text;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter(SyntaxTree tree) : ISyntaxVisitor
{
    private const int MaxBlankLines = 2;
    private const int MaxNewLines = MaxBlankLines + 1;
    
    private readonly TriviaStream _trivia = new(tree.Trivia);
    private readonly StringBuilder _builder = new(tree.Source.Text.Length);
    private readonly Stack<ISyntax> _blockContext = new();
    public int Depth { get; set; }

    private bool _pendingSpace;
    private bool _pendingIndent;
    private int _requiredNewLines;
    private int _writtenNewLines;

    private void Space() => _pendingSpace = true;
    private void Indent() => _pendingIndent = true;

    private void Write(Lexer<Token>.Token token)
    {
        WriteTrivia(token);
        FlushLayout();

        _builder.Append(tree.Source.GetText(token.Offset, token.Length));

        _writtenNewLines = 0;
        _pendingIndent = false;
    }

    private void WriteLine(Lexer<Token>.Token token)
    {
        Write(token);
        _requiredNewLines = Math.Max(_requiredNewLines, 1);
    }

    private void FlushLayout()
    {
        if (_requiredNewLines > 0)
        {
            _pendingSpace = false;

            EnsureNewLines(_requiredNewLines);
            _requiredNewLines = 0;
        }

        if (_writtenNewLines > 0 || _builder.Length == 0)
        {
            _pendingSpace = false;

            if (_pendingIndent)
                _builder.Append('\t', Depth);
        }
        else if (_pendingSpace)
        {
            _builder.Append(' ');
        }

        _pendingSpace = false;
    }

    private void EnsureNewLines(int count)
    {
        _pendingSpace = false;

        count = Math.Min(count, MaxNewLines);

        while (_writtenNewLines < count)
        {
            _builder.Append(Environment.NewLine);
            _writtenNewLines++;
        }
    }

    private void WriteTrivia(Lexer<Token>.Token token)
    {
        foreach (var trivia in _trivia.Read(token))
        {
            if (trivia is null) continue;

            switch (trivia.Value.Type)
            {
                case Token.COMMENT:
                {
                    WriteComment(trivia.Value);
                    break;
                }

                case Token.NEWLINE:
                {
                    var count = trivia.Value.Length / Environment.NewLine.Length;

                    EnsureNewLines(Math.Max(_requiredNewLines, count));

                    _requiredNewLines = 0;

                    break;
                }

                case Token.WHITESPACE:
                {
                    Space();
                    break;
                }
            }
        }
    }

    private void WriteComment(Lexer<Token>.Token token)
    {
        var text = tree.Source.GetText(token.Offset, token.Length);

        if (_writtenNewLines > 0 || _builder.Length == 0)
        {
            _pendingSpace = false;

            if (_pendingIndent)
                _builder.Append('\t', Depth);
        }
        else if (_pendingSpace)
        {
            _builder.Append(' ');
            _pendingSpace = false;
        }

        switch (text)
        {
            case ['/', '/', ..]:
            {
                _builder.Append(text);
                _writtenNewLines = 0;

                EnsureNewLines(Math.Max(_requiredNewLines, 1));

                _requiredNewLines = 0;

                break;
            }

            case ['/', '*', ..]:
            {
                _builder.Append(text);
                _writtenNewLines = 0;
                break;
            }
        }
    }

    private void Finish()
    {
        _pendingSpace = false;

        EnsureNewLines(_requiredNewLines);

        _requiredNewLines = 0;
    }

    public static string Format(SyntaxTree tree)
    {
        var formatter = new Formatter(tree);

        foreach (var root in tree.Roots)
            root.Accept(formatter);

        formatter.Finish();

        return formatter._builder.ToString();
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

        WriteLine(node.RightBrace);
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
        Indent();
        Write(node.ElseKeyword);
        Space();
        Write(node.IfKeyword);
        Space();
        Write(node.LeftParenthesis);
        node.Condition.Accept(this);
        Write(node.RightParenthesis);
        Space();
        node.Body.Accept(this);
        node.Next?.Accept(this);
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
            node.Initializer?.Accept(this);
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
        node.ElseIf?.Accept(this);

        if (node.Else is { } @else)
        {
            Indent();
            Write(node.ElseKeyword!.Value);
            Space();
            @else.Accept(this);
        }

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