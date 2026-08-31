using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class CompletionWalker(SyntaxTree tree, Position position) : ISyntaxVisitor
{
    private SymbolRoot SymbolRoot => tree.SymbolRoot;
    private ISymbolTable Symbols => tree.SymbolRoot.Current;
    private Stack<CompletionRange> CompletionScope { get; } = [];
    private List<CompletionItem> Completions { get; } = [];

    public static List<CompletionItem> Execute(SyntaxTree tree, Position position)
    {
        var walker = new CompletionWalker(tree, position);

        foreach (var root in tree.Roots)
            root.Accept(walker);

        return walker.Completions;
    }

    public void Visit(AssignmentExpression node) { }

    public void Visit(BinaryExpression node) { }

    public void Visit(BlockStatement node)
    {
        SymbolRoot.Push(node);
        CompletionScope.Push(SyntaxRangeProvider.Instance.Visit(node));

        foreach (var statement in node.Body)
            statement.Accept(this);

        CompletionScope.Pop();
        SymbolRoot.Pop();
    }

    public void Visit(BoolLiteralExpression node) { }

    public void Visit(ConstStatement node)
    {
        if (Symbols.Parent is null || CompletionScope.Peek().Contains(position))
        {
            Completions.Add(new CompletionItem
            {
                Label = node.Symbol,
                Kind = CompletionItemKind.Constant,
            });
        }
    }

    public void Visit(ElseIfStatement node)
    {
        node.Body.Accept(this);
        node.Next?.Accept(this);
    }

    public void Visit(ExpressionStatement node) { }

    public void Visit(ForStatement node)
    {
        SymbolRoot.Push(node);
        CompletionScope.Push(SyntaxRangeProvider.Instance.Visit(node));

        node.Initializer?.Accept(this);
        node.Body.Accept(this);

        CompletionScope.Pop();
        SymbolRoot.Pop();
    }

    public void Visit(GroupingExpression node) { }

    public void Visit(IdentifierExpression node) { }

    public void Visit(IfStatement node) => node.Body.Accept(this);

    public void Visit(NumericLiteralExpression node) { }

    public void Visit(PrintStatement node) { }

    public void Visit(StringLiteralExpression node) { }

    public void Visit(UnaryExpression node) { }

    public void Visit(VarStatement node)
    {
        if (Symbols.Parent is null || CompletionScope.Peek().Contains(position))
        {
            Completions.Add(new CompletionItem
            {
                Label = node.Symbol,
                Kind = CompletionItemKind.Variable,
            });
        }
    }

    public void Visit(WhileStatement node) => node.Body.Accept(this);
    
    public void Visit(BreakStatement node) { }
    
    public void Visit(ContinueStatement node) { }
}