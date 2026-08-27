using System.Runtime.InteropServices;

namespace Imparsable.Tools.Virtualization.UnitTests;

public class StackTests
{
    [Fact]
    public void Stack_Initializes_Correct_Number_Of_Slots()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Assert
        Assert.Equal(2, stack.Slots.Length);
    }

    [Fact]
    public void Stack_Push_Correctly_Pushes_Value()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Act
        stack.Push(StackSlot.FromNumber(1));

        // Assert
        Assert.Equal(1, stack.ActiveSlots.Length);
        Assert.Equal(StackSlot.FromNumber(1), stack.Slots[0]);
    }

    [Fact]
    public void Stack_Push_Throws_StackOverflowException_When_Stack_Is_Full()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);
        stack.Push(StackSlot.FromNumber(0));
        stack.Push(StackSlot.FromNumber(1));

        // Act & assert
        Assert.Throws<StackOverflowException>(() => stack.Push(StackSlot.FromNumber(2)));
    }

    [Fact]
    public void Stack_Pop_Correctly_Pops_Value()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Act
        stack.Push(StackSlot.FromNumber(1));
        var value = stack.Pop();

        // Assert
        Assert.Equal(0, stack.ActiveSlots.Length);
        Assert.Equal(1, value.Number);
    }

    [Fact]
    public void Stack_Pop_Throws_InvalidOperationException_When_Stack_Is_Empty()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Act & assert
        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void Stack_Peek_Correctly_Peeks_Value()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Act
        stack.Push(StackSlot.FromNumber(1));
        ref var value = ref stack.Peek();

        // Assert
        Assert.Equal(1, stack.ActiveSlots.Length);
        Assert.Equal(1, value.Number);
    }

    [Fact]
    public void Stack_Peek_Throws_InvalidOperationException_When_Stack_Is_Empty()
    {
        // Arrange
        var stack = new Stack<StackSlot>(new byte[Marshal.SizeOf<StackSlot>() * 2]);

        // Act & assert
        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }
}