using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using Imparsable.Lang.Calculator.Compilation;
using Imparsable.Tools.Compilation;

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

    private static StringConversion ReadStringConversion(ref int ip, ref Chunk chunk) =>
        (StringConversion)chunk.Code[ip++];

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
                // var right = Memory.Stack.Pop();
                // var left = Memory.Stack.Pop();
                // var lhsBytes = Memory.Heap.GetBytes(left.Reference);
                // var rhsBytes = Memory.Heap.GetBytes(right.Reference);
                // var leftString = Encoding.UTF8.GetString(lhsBytes[sizeof(int)..]);
                // var rightString = Encoding.UTF8.GetString(rhsBytes[sizeof(int)..]);
                // var concatenatedString = leftString + rightString;
                // var handle = Memory.StringHeap.Allocate(concatenatedString);* 2
                // Memory.Stack.Push(StackSlot.FromString(handle));

                // var right = Memory.Stack.Pop();
                // var left = Memory.Stack.Pop();
                // var lhsBytes = Memory.Heap.GetBytes(left.Reference);
                // var rhsBytes = Memory.Heap.GetBytes(right.Reference);
                // var lhsLength = BinaryPrimitives.ReadInt32LittleEndian(lhsBytes[..sizeof(int)]);
                // var rhsLength = BinaryPrimitives.ReadInt32LittleEndian(rhsBytes[..sizeof(int)]);
                // var lhsCharCount = Encoding.UTF8.GetCharCount(lhsBytes[sizeof(int)..(sizeof(int) + lhsLength)]);
                // var rhsCharCount = Encoding.UTF8.GetCharCount(rhsBytes[sizeof(int)..(sizeof(int) + rhsLength)]);
                //
                // using var buffer = CharBuffer.Acquire(lhsCharCount + rhsCharCount);
                // Encoding.UTF8.GetChars(lhsBytes[sizeof(int)..(sizeof(int) + lhsLength)], buffer.Span[..lhsLength]);
                // Encoding.UTF8.GetChars(rhsBytes[sizeof(int)..(sizeof(int) + rhsLength)], buffer.Span[lhsLength..(lhsLength + rhsLength)]);
                //
                // var handle = Memory.StringHeap.Allocate(buffer.Span);
                // Memory.Stack.Push(StackSlot.FromString(handle));

                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();

                var lhs = Memory.StringHeap.GetValueUtf8(left.Reference);
                var rhs = Memory.StringHeap.GetValueUtf8(right.Reference);

                var handle = Memory.StringHeap.Allocate(lhs, rhs);

                Memory.Stack.Push(StackSlot.FromString(handle));
                break;
            }

            case OpCode.EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                // Hacky, but works because of type checking.
                // At this point we know the types are equal,
                // so comparing the values as doubles means comparing the entire byte sequence.
                // This will cover all equality operations for now.
                // TODO: Fix this!
                Memory.Stack.Push(StackSlot.FromBool(left.Number.Equals(right.Number)));
                break;
            }

            case OpCode.NOT_EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                // Hacky, but works because of type checking.
                // At this point we know the types are equal,
                // so comparing the values as doubles means comparing the entire byte sequence.
                // This will cover all equality operations for now.
                // TODO: Fix this!
                Memory.Stack.Push(StackSlot.FromBool(!left.Number.Equals(right.Number)));
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
                var conversion = ReadStringConversion(ref ip, ref chunk);
                var @string = conversion switch
                {
                    StringConversion.BOOL => Memory.Stack.Pop().Bool.ToString(),
                    StringConversion.NUMBER => Memory.Stack.Pop().Number.ToString(CultureInfo.InvariantCulture),
                    _ => throw new InvalidOperationException()
                };
                var handle = Memory.StringHeap.Allocate(@string);
                Memory.Stack.Push(StackSlot.FromString(handle));

                break;
            }

            case OpCode.PRINT:
            {
                // var handle = Memory.Stack.Pop();
                // var bytes = Memory.Heap.GetBytes(handle.Reference);
                // var length = BinaryPrimitives.ReadInt32LittleEndian(bytes[..sizeof(int)]);
                // var value = Encoding.UTF8.GetString(bytes[sizeof(int)..(sizeof(int) + length)]);
                // StdOut.Invoke(value);

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