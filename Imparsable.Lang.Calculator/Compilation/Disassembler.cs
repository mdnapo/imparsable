using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed class Disassembler(SyntaxTree tree, DiagnosticsProvider diagnostics) : CompilerBase(tree, diagnostics)
{
    private readonly Dictionary<int, string> _constants = [];
    
    private sealed record Instruction(int Offset, int Indent, OpCode OpCode)
    {
        public List<Operand> Operands { get; } = [];
    }

    private abstract record Operand;

    private sealed record ValueOperand(string Value) : Operand;
    
    private sealed record EnumOperand<T>(T Value) : Operand where T : unmanaged;

    private sealed record JumpOperand(int Offset) : Operand
    {
        public int GetTarget(Instruction instruction) =>
            instruction.Offset + sizeof(byte) + sizeof(int) + Offset;
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

                switch (operand)
                {
                    case ValueOperand value:
                        builder.Append(value.Value);
                        break;
                    
                    case EnumOperand<BoolValue> value:
                        builder.Append(value.Value);
                        break;

                    case EnumOperand<StringConversion> value:
                        builder.Append(value.Value);
                        break;

                    case JumpOperand jump:
                        builder.Append(jump.Offset.ToString("+#;-#;0"));
                        builder.Append(" -> ");
                        builder.Append(jump.GetTarget(instruction).ToString("000000"));
                        break;
                }
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

        if (_current is null)
            return;

        if (_current.OpCode == OpCode.NUM_CONST)
        {
            var constants = CollectionsMarshal.AsSpan(Constants);
            var number = BinaryPrimitives.ReadDoubleLittleEndian(
                constants.Slice(value, sizeof(double))
            );

            _current.Operands.Add(new ValueOperand($"{value} ({number})"));
            return;
        }
        
        if (_current.OpCode == OpCode.STRING_CONST)
        {
            var constants = CollectionsMarshal.AsSpan(Constants);
            var span = constants[value..];

            var length = BinaryPrimitives.ReadInt32LittleEndian(span[..sizeof(int)]);
            var text = Encoding.UTF8.GetString(span.Slice(sizeof(int), length));

            _current.Operands.Add(new ValueOperand($"{value} (\"{text}\")"));
            return;
        }

        _current.Operands.Add(new ValueOperand(value.ToString()));
    }
    
    public override void EmitByte<TValue>(TValue value)
    {
        base.EmitByte(value);
        _current?.Operands.Add(new EnumOperand<TValue>(value));
    }

    public override int EmitJump(OpCode instruction)
    {
        FlushInstruction();

        var entry = new Instruction(Offset: Code.Count, Indent: Indent, OpCode: instruction);

        entry.Operands.Add(new JumpOperand(0));

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
        instruction.Operands.Add(new JumpOperand(jump));
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