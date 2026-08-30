using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Toolchain.Compilation;

namespace Imparsable.Lang.Calculator.Virtualization;

public class VirtualMachine : IDisposable
{
    public event Action<string> StdOut = delegate { };
    public Memory Memory { get; } = new();

    public void Execute(Chunk chunk)
    {
        var ip = 0;

        while (ip < chunk.Code.Length)
        {
            var op = (OpCode)chunk.Code[ip++];
            Execute(ref ip, ref chunk, op);
        }
    }

    private static int ReadInt32(ref int ip, ref Chunk chunk) =>
        BinaryPrimitives.ReadInt32LittleEndian(chunk.Code[ip..(ip += sizeof(int))]);

    private static T ReadByte<T>(ref int ip, ref Chunk chunk) where T : unmanaged =>
        MemoryMarshal.Read<T>(chunk.Code[ip..++ip]);

    private void Execute(ref int ip, ref Chunk chunk, OpCode op)
    {
        switch (op)
        {
            case OpCode.GET_LOCAL:
            {
                var index = ReadInt32(ref ip, ref chunk);
                var value = Memory.Stack.Slots[index];
                Memory.Stack.Push(value);
                break;
            }

            case OpCode.SET_LOCAL:
            {
                var index = ReadInt32(ref ip, ref chunk);
                Memory.Stack.Slots[index] = Memory.Stack.Peek();
                break;
            }

            case OpCode.POP:
            {
                Memory.Stack.Pop();
                break;
            }

            case OpCode.ADD:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromNumber(left.Number + right.Number));
                break;
            }

            case OpCode.SUB:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromNumber(left.Number - right.Number));
                break;
            }

            case OpCode.MUL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromNumber(left.Number * right.Number));
                break;
            }

            case OpCode.DIV:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromNumber(left.Number / right.Number));
                break;
            }

            case OpCode.CONCAT:
            {
                Memory.CollectGarbage();

                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                var lhs = Memory.StringHeap.GetValueUtf8(left.Reference);
                var rhs = Memory.StringHeap.GetValueUtf8(right.Reference);
                var handle = Memory.StringHeap.Allocate(lhs, rhs);
                Memory.Stack.Push(StackSlot.FromString(handle));
                break;
            }

            case OpCode.OR:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Bool || right.Bool));
                break;
            }

            case OpCode.AND:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Bool && right.Bool));
                break;
            }

            case OpCode.EQUAL:
            {
                var equality = ReadByte<EqualityType>(ref ip, ref chunk);
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();

                switch (equality)
                {
                    case EqualityType.BOOL:
                        Memory.Stack.Push(StackSlot.FromBool(left.Bool == right.Bool));
                        break;

                    case EqualityType.NUMBER:
                        Memory.Stack.Push(StackSlot.FromBool(Math.Abs(left.Number - right.Number) < 0.01));
                        break;

                    case EqualityType.STRING:
                        var lhs = Memory.StringHeap.GetValueUtf8(left.Reference);
                        var rhs = Memory.StringHeap.GetValueUtf8(right.Reference);
                        Memory.Stack.Push(StackSlot.FromBool(lhs.SequenceEqual(rhs)));
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                break;
            }

            case OpCode.NOT_EQUAL:
            {
                var equality = ReadByte<EqualityType>(ref ip, ref chunk);
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();

                switch (equality)
                {
                    case EqualityType.BOOL:
                        Memory.Stack.Push(StackSlot.FromBool(!left.Bool == right.Bool));
                        break;

                    case EqualityType.NUMBER:
                        Memory.Stack.Push(StackSlot.FromBool(!(Math.Abs(left.Number - right.Number) < 0.01)));
                        break;

                    case EqualityType.STRING:
                        var lhs = Memory.StringHeap.GetValueUtf8(left.Reference);
                        var rhs = Memory.StringHeap.GetValueUtf8(right.Reference);
                        Memory.Stack.Push(StackSlot.FromBool(!lhs.SequenceEqual(rhs)));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                break;
            }

            case OpCode.LOWER_THAN:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Number < right.Number));
                break;
            }

            case OpCode.LOWER_EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Number <= right.Number));
                break;
            }

            case OpCode.GREATER_THAN:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Number > right.Number));
                break;
            }

            case OpCode.GREATER_EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(left.Number >= right.Number));
                break;
            }

            case OpCode.BOOL_CONST:
            {
                var value = ReadByte<BoolValue>(ref ip, ref chunk);
                Memory.Stack.Push(StackSlot.FromBool(Unsafe.As<BoolValue, bool>(ref value)));
                break;
            }

            case OpCode.NUM_CONST:
            {
                var constantIndex = ReadInt32(ref ip, ref chunk);
                var constant = chunk.Constants[constantIndex..(constantIndex + sizeof(double))];
                var value = BinaryPrimitives.ReadDoubleLittleEndian(constant);
                Memory.Stack.Push(StackSlot.FromNumber(value));
                break;
            }

            case OpCode.STRING_CONST:
            {
                Memory.CollectGarbage();

                var index = ReadInt32(ref ip, ref chunk);
                var header = index + sizeof(int);
                var length = BinaryPrimitives.ReadInt32LittleEndian(chunk.Constants[index..header]);
                var value = chunk.Constants[header..(header + length)];
                var handle = Memory.StringHeap.Allocate(value);
                Memory.Stack.Push(StackSlot.FromString(handle));
                break;
            }

            case OpCode.NEGATE_BOOL:
            {
                var value = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromBool(!value.Bool));
                break;
            }

            case OpCode.NEGATE_NUM:
            {
                var value = Memory.Stack.Pop();
                Memory.Stack.Push(StackSlot.FromNumber(-value.Number));
                break;
            }

            case OpCode.JMP:
            {
                var jmp = ReadInt32(ref ip, ref chunk);
                ip += jmp;
                break;
            }

            case OpCode.JMP_FALSE:
            {
                var jmp = ReadInt32(ref ip, ref chunk);
                if (!Memory.Stack.Peek().Bool)
                {
                    ip += jmp;
                }

                break;
            }

            case OpCode.TO_STRING:
            {
                Memory.CollectGarbage();

                var conversion = ReadByte<StringConversion>(ref ip, ref chunk);
                var value = Memory.Stack.Pop();
                var @string = conversion switch
                {
                    StringConversion.BOOL => value.Bool ? "true" : "false",
                    StringConversion.NUMBER => value.Number.ToString(CultureInfo.InvariantCulture),
                    _ => throw new InvalidOperationException()
                };

                var handle = Memory.StringHeap.Allocate(@string);
                Memory.Stack.Push(StackSlot.FromString(handle));
                break;
            }

            case OpCode.PRINT:
            {
                var handle = Memory.Stack.Pop();
                var value = Memory.StringHeap.GetValueUtf8(handle.Reference);
                StdOut.Invoke(Encoding.UTF8.GetString(value));
                break;
            }

            default:
                throw new InvalidOperationException("Unknown opcode: " + op);
        }
    }

    public void Dispose()
    {
        foreach (var @delegate in StdOut.GetInvocationList())
            StdOut -= @delegate as Action<string>;
    }
}