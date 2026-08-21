using System.Buffers.Binary;
using System.Text;
using Imparsable.Tool.Calculator.Execution;
using Imparsable.Tool.Calculator.Syntax;
using Imparsable.Virtualization;

namespace Imparsable.Tool.Calculator;

public class Compiler(SyntaxTree tree) : Compiler<OpCode>, ISyntaxVisitor
{
    private static readonly ReadOnlyMemory<byte> FalseBytes = BitConverter.GetBytes((double)0);
    private static readonly ReadOnlyMemory<byte> TrueBytes = BitConverter.GetBytes((double)1);
    private static readonly NumericLiteralExpression ZeroValue = new() { Token = default, Value = 0 };

    private readonly Stack<SymbolTable> _symbolTables = new([tree.SymbolTable]);
    private readonly Stack<List<int>> _elseJumps = [];

    private SymbolTable CurrentSymbolTable => _symbolTables.Peek();

    public static Chunk Execute(SyntaxTree tree)
    {
        var compiler = new Compiler(tree);
        foreach (var node in tree.Roots)
            node.Accept(compiler);

        return new Chunk(compiler.Code.ToArray(), compiler.Constants.ToArray());
    }

    private void BeginScope(SymbolTable symbolTable) => _symbolTables.Push(symbolTable);

    private void EndScope()
    {
        foreach (var _ in CurrentSymbolTable.Symbols)
            EmitOpCode(OpCode.POP);

        _symbolTables.Pop();
    }

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);

        var text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);

        var op = node.Operator.Type switch
        {
            Token.PLUS => OpCode.ADD,
            Token.MINUS => OpCode.SUB,
            Token.STAR => OpCode.MUL,
            Token.SLASH => OpCode.DIV,
            Token.LOWER_THAN => OpCode.LOWER_THAN,
            Token.LOWER_EQUAL => OpCode.LOWER_EQUAL,
            Token.GREATER_THAN => OpCode.GREATER_THAN,
            Token.GREATER_EQUAL => OpCode.GREATER_EQUAL,
            Token.EQUAL_EQUAL => OpCode.EQUAL,
            Token.BANG_EQUAL => OpCode.NOT_EQUAL,
            Token.DOT => OpCode.CONCAT,
            _ => throw new InvalidOperationException($"Unsupported operator '{text}' for binary expression.")
        };

        EmitOpCode(op);
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        EmitOpCode(OpCode.SET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        EmitOpCode(OpCode.POP);
    }

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        EmitOpCode(OpCode.GET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(NumericLiteralExpression node)
    {
        var buffer = ByteBuffer.Acquire(sizeof(double));
        var span = buffer.Span;

        BinaryPrimitives.WriteDoubleLittleEndian(span, node.Value);
        EmitOpCode(OpCode.NUM_CONST);
        var index = AddConstant(span);
        EmitInt32(index);
    }

    public void Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        EmitOpCode(OpCode.PRINT);
    }

    public void Visit(StringLiteralExpression node)
    {
        var @string = node.Value.Trim('\'', '"');
        var length = @string.Length;
        var size = sizeof(int) + length;
        var buffer = ByteBuffer.Acquire(size);
        var span = buffer.Span;

        BinaryPrimitives.WriteInt32LittleEndian(span[..sizeof(int)], length);
        Encoding.UTF8.GetBytes(@string, span[sizeof(int)..]);

        EmitOpCode(OpCode.STRING_CONST);
        var index = AddConstant(span);
        EmitInt32(index);
    }

    public void Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);
        var text = tree.Source.GetText(node.Token.Offset, node.Token.Length);

        var op = node.Op.Type switch
        {
            Token.MINUS => OpCode.NEGATE_NUM,
            Token.BANG => OpCode.NEGATE_BOOL,
            _ => throw new InvalidOperationException($"Unsupported operator '{text}' for binary expression.")
        };

        EmitOpCode(op);
    }

    public void Visit(VarStatement node)
    {
        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
        }
        else
        {
            ZeroValue.Accept(this);
        }

        EmitOpCode(OpCode.SET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(IfStatement node)
    {
        node.Condition.Accept(this);

        var thenJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        _elseJumps.Push([EmitJump(OpCode.JMP)]);

        PatchJump(thenJump);

        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);

        foreach (var jump in _elseJumps.Peek())
            PatchJump(jump);

        _elseJumps.Pop();
    }

    public void Visit(BlockStatement node)
    {
        BeginScope(node.SymbolTable);
        foreach (var statement in node.Body)
            statement.Accept(this);
        EndScope();
    }

    public void Visit(ElseIfStatement node)
    {
        EmitOpCode(OpCode.POP);

        node.Condition.Accept(this);

        var thenJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        _elseJumps.Peek().Add(EmitJump(OpCode.JMP));

        PatchJump(thenJump);

        node.Next?.Accept(this);
    }

    public void Visit(ForStatement node)
    {
        BeginScope(node.SymbolTable);

        node.Initializer?.Accept(this);

        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        if (node.Increment is not null)
        {
            node.Increment.Accept(this);
            EmitOpCode(OpCode.POP);
        }

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);

        EndScope();
    }

    public void Visit(WhileStatement node)
    {
        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);
    }

    public void Visit(BoolLiteralExpression node)
    {
        EmitOpCode(OpCode.NUM_CONST);
        var index = AddConstant(node.Value ? TrueBytes.Span : FalseBytes.Span);
        EmitInt32(index);
    }

    public void Visit(AssignmentExpression node)
    {
        if (node.Target is IdentifierExpression identifier)
        {
            node.Value.Accept(this);
            var offset = CurrentSymbolTable.Offset(identifier.Symbol);

            if (node.Operator.Type is Token.PLUS_EQUAL or Token.MINUS_EQUAL or Token.STAR_EQUAL or Token.SLASH_EQUAL)
            {
                EmitOpCode(OpCode.GET_LOCAL);
                EmitInt32(offset);

                var op = node.Operator.Type switch
                {
                    Token.PLUS_EQUAL => OpCode.ADD,
                    Token.MINUS_EQUAL => OpCode.SUB,
                    Token.STAR_EQUAL => OpCode.MUL,
                    Token.SLASH_EQUAL => OpCode.DIV,
                    _ => throw new InvalidOperationException($"Unsupported assignment operator '{node.Operator.Type}'.")
                };

                EmitOpCode(op);
            }

            EmitOpCode(OpCode.SET_LOCAL);
            EmitInt32(offset);
        }

        else throw new InvalidOperationException($"Assignment target of type '{node.GetType()}' is not supported.");
    }
}