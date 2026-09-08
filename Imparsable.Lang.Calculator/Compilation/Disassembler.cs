using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using Imparsable.Lang.Calculator.Exceptions;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Toolchain;
using Imparsable.Toolchain.Extensions;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler(SyntaxTree tree, DiagnosticsProvider diagnostics) : Compiler(tree, diagnostics)
{
    private readonly Dictionary<int, string> _constants = [];
    private readonly List<Entry> _entries = [];
    private readonly Dictionary<int, Instruction> _jumps = [];
    private Instruction? _current;
    private int Indent { get; set; } = 1;

    public static string? Disassemble(SyntaxTree tree, DiagnosticsProvider diagnostics) =>
        new Disassembler(tree, diagnostics).Disassemble();

    public string? Disassemble()
    {
        try
        {
            WalkTree();

            var builder = new StringBuilder();

            WriteConstants(builder);

            builder.AppendLine();

            WriteCode(builder);

            return builder.ToString();
        }
        catch (HaltException)
        {
            return null;
        }
    }

    private void WriteConstants(StringBuilder builder)
    {
        if (_constants.Count == 0)
            return;

        builder.AppendLine("constants:");

        foreach (var (offset, value) in _constants.OrderBy(x => x.Key))
            builder.AppendLine($" {offset:000000} {value}");
    }

    private void WriteCode(StringBuilder builder)
    {
        if (_entries.Count == 0)
            return;

        builder.AppendLine("code:");

        foreach (var entry in _entries)
            entry.WriteTo(builder);
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
        _current?.Operands.Add(GetOperand(value));
    }

    public override void EmitByte<TValue>(TValue value)
    {
        base.EmitByte(value);
        _current?.Operands.Add(new EnumOperand<TValue>(value));
    }

    public override int EmitJump(OpCode instruction)
    {
        var offset = base.EmitJump(instruction);

        if (_current is null)
            throw new InvalidOperationException("Missing jump instruction.");

        _current.Operands.Clear();
        _current.Operands.Add(new JumpOperand(0));

        _jumps[offset] = _current;

        return offset;
    }

    public override void EmitLoop(OpCode jump, int loopStart)
    {
        base.EmitLoop(jump, loopStart);

        if (_current is null)
            throw new InvalidOperationException("Missing loop instruction.");

        var offset = BinaryPrimitives.ReadInt32LittleEndian(
            Code.Span.Slice(Code.Count - sizeof(int), sizeof(int))
        );

        _current.Operands.Clear();
        _current.Operands.Add(new JumpOperand(offset));
    }

    public override void PatchJump(int offset)
    {
        base.PatchJump(offset);

        var jump = BinaryPrimitives.ReadInt32LittleEndian(
            Code.Span.Slice(offset, sizeof(int))
        );

        var instruction = _jumps[offset];
        instruction.Operands.Clear();
        instruction.Operands.Add(new JumpOperand(jump));
    }

    public override void Visit(BlockStatement node)
    {
        AddLabel("block:", () => base.Visit(node));
    }

    public override void Visit(IfStatement node)
    {
        AddLabel("if:", () => base.Visit(node));
    }

    public override void Visit(ElseIfStatement node)
    {
        AddLabel("else if:", () => base.Visit(node));
    }

    public override void Visit(ForStatement node)
    {
        AddLabel("for:", () => base.Visit(node));
    }

    public override void Visit(WhileStatement node)
    {
        AddLabel("while:", () => base.Visit(node));
    }

    private ValueOperand GetOperand(int value) => _current?.OpCode switch
    {
        OpCode.NUM_CONST => GetNumberConstant(value),
        OpCode.STRING_CONST => GetStringConstant(value),
        _ => new ValueOperand(value.ToString())
    };

    private ValueOperand GetNumberConstant(int offset)
    {
        var value = BinaryPrimitives.ReadDoubleLittleEndian(
            Constants.Span.Slice(offset, sizeof(double))
        );

        _constants[offset] = value.ToString(CultureInfo.InvariantCulture);

        return new ValueOperand($"(offset: {offset}, value: {value})");
    }

    private ValueOperand GetStringConstant(int offset)
    {
        var span = Constants.Span[offset..];

        var length = BinaryPrimitives.ReadInt32LittleEndian(span[..sizeof(int)]);
        var value = Encoding.UTF8.GetString(span.Slice(sizeof(int), length));

        _constants[offset] = $"\"{value}\"";

        return new ValueOperand($"(offset: {offset}, value: \"{value}\")");
    }

    private void AddLabel(string name, Action action)
    {
        FlushInstruction();
        _entries.Add(new Label(Indent: Indent, Name: name));

        Indent++;
        action();
        Indent--;
    }

    private void FlushInstruction()
    {
        if (_current is null) return;

        _entries.Add(_current);
        _current = null;
    }
}