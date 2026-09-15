using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class HoverProvider(SyntaxTree tree, Position position) : ISyntaxVisitor
{
    private SymbolRoot SymbolRoot => tree.SymbolRoot;
    private ISymbolTable Symbols => SymbolRoot.Current;

    public Hover? Hover { get; private set; }

    public static Hover? Execute(SyntaxTree tree, Position position)
    {
        var provider = new HoverProvider(tree, position);

        foreach (var root in tree.Roots)
        {
            root.Accept(provider);

            if (provider.Hover is not null)
                break;
        }

        return provider.Hover;
    }
    
    private bool Contains(ISyntax syntax) =>
        syntax.Accept(SyntaxRangeProvider.Instance).Contains(position);

    public void Visit(AssignmentExpression node)
    {
        node.Target.Accept(this);
        node.Value.Accept(this);
    }

    public void Visit(BinaryExpression node)
    {
        if (!Contains(node)) return;
        
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);
    }

    public void Visit(BlockStatement node)
    {
        SymbolRoot.Push(node);

        foreach (var statement in node.Body)
        {
            statement.Accept(this);

            if (Hover is not null)
                break;
        }

        SymbolRoot.Pop();
    }

    public void Visit(BoolLiteralExpression node) { }

    public void Visit(ConstStatement node) => node.Initializer.Accept(this);

    public void Visit(ElseIfStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
        node.Next?.Accept(this);
    }

    public void Visit(ExpressionStatement node)
    {
        if (!Contains(node)) return;
        
        node.Expression.Accept(this);
    }

    public void Visit(ForStatement node)
    {
        SymbolRoot.Push(node);

        node.Initializer?.Accept(this);
        node.Condition.Accept(this);
        node.Increment?.Accept(this);
        node.Body.Accept(this);

        SymbolRoot.Pop();
    }

    public void Visit(GroupingExpression node)
    {
        if (!Contains(node)) return;

        node.Expression.Accept(this);
    }

    public void Visit(IdentifierExpression node)
    {
        if (!SyntaxRange.From(node.Token).Contains(position))
            return;

        if (Symbols.RecursiveLookup(node.Symbol) is not ISyntax symbol)
            return;

        if (!tree.Types.TryGetValue(symbol, out var type))
            return;

        var kind = symbol switch
        {
            ConstStatement => "const",
            VarStatement => "var",
            _ => "unknown"
        };

        var text = $"{kind} {tree.Source.GetTextSpan(node.Symbol)}: {type}";

        Hover = new Hover
        {
            Contents = new(text),
            Range = node.Token.ToRange()
        };
    }

    public void Visit(IfStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);
    }

    public void Visit(NumericLiteralExpression node) { }

    public void Visit(PrintStatement node)
    {
        if (!Contains(node)) return;
        
        node.Expression.Accept(this);
    }

    public void Visit(StringLiteralExpression node) { }

    public void Visit(UnaryExpression node) => node.Operand.Accept(this);

    public void Visit(VarStatement node) => node.Initializer?.Accept(this);

    public void Visit(WhileStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
    }

    public void Visit(BreakStatement node) { }

    public void Visit(ContinueStatement node) { }

    public void Visit(ErrorNode node) { }
}