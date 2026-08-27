using System.Buffers.Binary;

namespace Imparsable.Tools.Compilation.UnitTests;

public class CompilerTests
{
    private enum OpCode : byte
    {
        NOP = 0x01,
        JMP = 0x02,
    }

    private enum InvalidOpCode : int;

    private sealed class TestCompiler : Compiler<OpCode>;

    private sealed class InvalidCompiler : Compiler<InvalidOpCode>;

    [Fact]
    public void Compiler_EmitOpCode_Correctly_Writes_Byte()
    {
        // Arrange
        var compiler = new TestCompiler();

        // Act
        compiler.EmitOpCode(OpCode.NOP);

        // Assert
        Assert.Equal([(byte)OpCode.NOP], compiler.Code);
    }

    [Fact]
    public void Compiler_EmitByte_Correctly_Writes_Byte()
    {
        // Arrange
        var compiler = new TestCompiler();

        // Act
        compiler.EmitByte(0x42);

        // Assert
        Assert.Equal([0x42], compiler.Code);
    }

    [Fact]
    public void Compiler_AddConstant_Returns_Correct_Offset()
    {
        // Arrange
        var compiler = new TestCompiler();

        compiler.AddConstant([0x01, 0x02, 0x03]);

        // Act
        var offset = compiler.AddConstant([0x04, 0x05]);

        // Assert
        Assert.Equal(3, offset);
    }

    [Fact]
    public void Compiler_AddConstant_Correctly_Appends_Value()
    {
        // Arrange
        var compiler = new TestCompiler();

        // Act
        compiler.AddConstant([0x01, 0x02]);
        compiler.AddConstant([0x03, 0x04]);

        // Assert
        Assert.Equal([0x01, 0x02, 0x03, 0x04], compiler.Constants);
    }

    [Fact]
    public void Compiler_EmitInt32_Correctly_Writes_Little_Endian_Value()
    {
        // Arrange
        var compiler = new TestCompiler();
        const int value = 0x12345678;

        // Act
        compiler.EmitInt32(value);

        // Assert
        Assert.Equal(
            [
                0x78,
                0x56,
                0x34,
                0x12,
            ],
            compiler.Code
        );
    }

    [Fact]
    public void Compiler_EmitJump_Correctly_Writes_Instruction_And_Placeholder()
    {
        // Arrange
        var compiler = new TestCompiler();

        // Act
        var offset = compiler.EmitJump(OpCode.JMP);

        // Assert
        Assert.Equal(1, offset);
        Assert.Equal(
            [
                (byte)OpCode.JMP,
                0x00,
                0x00,
                0x00,
                0x00,
            ],
            compiler.Code
        );
    }

    [Fact]
    public void Compiler_PatchJump_Correctly_Patches_Forward_Offset()
    {
        // Arrange
        var compiler = new TestCompiler();

        var offset = compiler.EmitJump(OpCode.JMP);

        compiler.EmitByte(0xAA);
        compiler.EmitByte(0xBB);
        compiler.EmitByte(0xCC);

        // Act
        compiler.PatchJump(offset);

        // Assert
        var jump = BinaryPrimitives.ReadInt32LittleEndian(
            compiler.Code.ToArray().AsSpan(offset, sizeof(int))
        );

        Assert.Equal(3, jump);
    }

    [Fact]
    public void Compiler_EmitLoop_Correctly_Writes_Backward_Offset()
    {
        // Arrange
        var compiler = new TestCompiler();
        var loopStart = compiler.Code.Count;

        compiler.EmitByte(0xAA);
        compiler.EmitByte(0xBB);
        compiler.EmitByte(0xCC);

        // Act
        compiler.EmitLoop(OpCode.JMP, loopStart);

        // Assert
        Assert.Equal((byte)OpCode.JMP, compiler.Code[3]);

        var offset = BinaryPrimitives.ReadInt32LittleEndian(
            compiler.Code.ToArray().AsSpan(4, sizeof(int))
        );

        Assert.Equal(-8, offset);
    }

    [Fact]
    public void Compiler_Build_Correctly_Creates_Chunk()
    {
        // Arrange
        var compiler = new TestCompiler();

        compiler.EmitOpCode(OpCode.NOP);
        compiler.AddConstant([0x01, 0x02, 0x03]);

        // Act
        var chunk = compiler.Build();

        // Assert
        Assert.Equal([(byte)OpCode.NOP], chunk.Code.ToArray());
        Assert.Equal([0x01, 0x02, 0x03], chunk.Constants.ToArray());
    }

    [Fact]
    public void Compiler_Invalid_Enum_Backing_Type_Throws()
    {
        // Act
        var exception = Record.Exception(() => new InvalidCompiler());

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<TypeInitializationException>(exception);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}