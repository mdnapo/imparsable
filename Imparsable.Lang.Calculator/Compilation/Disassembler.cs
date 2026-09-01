using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed class Disassembler(SyntaxTree tree, DiagnosticsProvider diagnostics) : CompilerBase(tree, diagnostics)
{
    private sealed record Instruction(int Offset, int Indent, OpCode OpCode)
    {
        public List<string> Operands { get; } = [];
    }

    private readonly List<Instruction> _instructions = [];
    private readonly Dictionary<int, Instruction> _jumps = [];
    private Instruction? _current;

    private int Indent { get; set; }

    public static string Disassemble(SyntaxTree tree, DiagnosticsProvider diagnostics)
    {
        var disassembler = new Disassembler(tree, diagnostics);
        disassembler.Visit();
        disassembler.FlushInstruction();

        var builder = new StringBuilder();

        foreach (var instruction in disassembler._instructions)
        {
            builder.Append(' ', instruction.Indent * 4);
            builder.Append(instruction.Offset.ToString("000000"));
            builder.Append('\t');
            builder.Append(instruction.OpCode);

            foreach (var operand in instruction.Operands)
            {
                builder.Append(' ');
                builder.Append(operand);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public void Visit()
    {
        try
        {
            Tree.SymbolRoot.Popped += OnPop;

            foreach (var node in Tree.Roots)
                node.Accept(this);
        }
        finally
        {
            Tree.SymbolRoot.Popped -= OnPop;
        }
    }

    public override void EmitOpCode(OpCode op)
    {
        FlushInstruction();
        _current = new Instruction(Offset: Code.Count, Indent: Indent, OpCode: op);
        base.EmitOpCode(op);
    }

    public override void EmitInt32(int value)
    {
        base.EmitInt32(value);
        _current?.Operands.Add(value.ToString());
    }

    public override void EmitByte<TValue>(TValue value)
    {
        base.EmitByte(value);
        _current?.Operands.Add(value.ToString()!);
    }

    public override int EmitJump(OpCode instruction)
    {
        FlushInstruction();

        var entry = new Instruction(Offset: Code.Count, Indent: Indent, OpCode: instruction);

        entry.Operands.Add("0");

        _instructions.Add(entry);

        var offset = base.EmitJump(instruction);

        _jumps[offset] = entry;

        return offset;
    }

    public override void PatchJump(int offset)
    {
        base.PatchJump(offset);

        var code = CollectionsMarshal.AsSpan(Code);
        var jump = BinaryPrimitives.ReadInt32LittleEndian(
            code.Slice(offset, sizeof(int))
        );

        var instruction = _jumps[offset];
        instruction.Operands.Clear();
        instruction.Operands.Add(jump.ToString());
    }

    public override void Visit(BlockStatement node)
    {
        Indent++;

        base.Visit(node);

        Indent--;
    }

    private void FlushInstruction()
    {
        if (_current is null)
            return;

        _instructions.Add(_current);
        _current = null;
    }
}