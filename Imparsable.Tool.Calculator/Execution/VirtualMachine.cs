using System.Buffers.Binary;
using System.Text;

namespace Imparsable.Tool.Calculator.Execution;

public class VirtualMachine : IDisposable
{
    public event Action<string> StdOut = delegate { };
    public VirtualMemory Memory { get; } = new();

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
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = left.Number + right.Number });
                break;
            }

            case OpCode.SUB:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = left.Number - right.Number });
                break;
            }

            case OpCode.MUL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = left.Number * right.Number });
                break;
            }

            case OpCode.DIV:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = left.Number / right.Number });
                break;
            }

            case OpCode.CONCAT:
            {
                var right = PopString();
                var left = PopString();
                var lhsBytes = Memory.Heap.GetBytes(left.String);
                var rhsBytes = Memory.Heap.GetBytes(right.String);
                var leftString = Encoding.UTF8.GetString(lhsBytes);
                var rightString = Encoding.UTF8.GetString(rhsBytes);
                var concatenatedString = leftString + rightString;
                var handle = Memory.StringHeap.Allocate(concatenatedString);
                Memory.Stack.Push(new StackSlot { Type = StackType.STRING, String = handle });
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
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = left.Number.Equals(right.Number) });
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
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = !left.Number.Equals(right.Number) });
                break;
            }

            case OpCode.LOWER_THAN:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = left.Number < right.Number });
                break;
            }

            case OpCode.LOWER_EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = left.Number <= right.Number });
                break;
            }

            case OpCode.GREATER_THAN:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = left.Number > right.Number });
                break;
            }

            case OpCode.GREATER_EQUAL:
            {
                var right = Memory.Stack.Pop();
                var left = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = left.Number >= right.Number });
                break;
            }

            case OpCode.NUM_CONST:
            {
                var constantIndex = ReadInt32(ref ip, ref chunk);
                var constant = chunk.Constants[constantIndex..(constantIndex + sizeof(double))];
                var value = BinaryPrimitives.ReadDoubleLittleEndian(constant);
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = value });
                break;
            }

            case OpCode.STRING_CONST:
            {
                var constantIndex = ReadInt32(ref ip, ref chunk);
                var bytes = chunk.Constants[constantIndex..(constantIndex + sizeof(int))];
                var stringLength = BinaryPrimitives.ReadInt32LittleEndian(bytes);
                var stringStart = constantIndex + sizeof(int);
                var stringEnd = constantIndex + sizeof(int) + stringLength;
                var value = Encoding.UTF8.GetString(chunk.Constants[stringStart..stringEnd]);

                var handle = Memory.StringHeap.Allocate(value);
                Memory.Stack.Push(new StackSlot { Type = StackType.STRING, String = handle });
                break;
            }

            case OpCode.NEGATE_BOOL:
            {
                var value = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.BOOL, Bool = !value.Bool });
                break;
            }

            case OpCode.NEGATE_NUM:
            {
                var value = Memory.Stack.Pop();
                Memory.Stack.Push(new StackSlot { Type = StackType.NUMBER, Number = -value.Number });
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

            case OpCode.PRINT:
            {
                var handle = PopString();
                var value = Encoding.UTF8.GetString(Memory.Heap.GetBytes(handle.String)[sizeof(int)..]);
                StdOut.Invoke(value);
                break;
            }

            default:
                throw new InvalidOperationException("Unknown opcode: " + op);
        }
    }

    private StackSlot PopString()
    {
        var value = Memory.Stack.Pop();

        if (value.Type == StackType.STRING) return value;

        var @string = value.Type switch
        {
            StackType.BOOL or StackType.NUMBER => value.ToString(),
            _ => throw new InvalidOperationException("Cannot convert stack type to string: " + value.Type)
        };

        var handle = Memory.StringHeap.Allocate(@string);

        return new StackSlot { Type = StackType.STRING, String = handle };
    }

    public void Dispose()
    {
        foreach (var @delegate in StdOut.GetInvocationList())
            StdOut -= @delegate as Action<string>;
    }
}