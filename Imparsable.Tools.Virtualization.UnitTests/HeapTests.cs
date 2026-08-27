namespace Imparsable.Tools.Virtualization.UnitTests;

public class HeapTests
{
    [Fact]
    public void Heap_Initializes_Correctly()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);

        // Assert
        Assert.Equal(0, heap.Allocations.Length);
    }

    [Fact]
    public void Heap_Allocate_Correctly_Allocates()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);

        // Act
        var handle = heap.Allocate(8, new Allocation());
        ref var allocation = ref heap.GetEntry(handle);

        // Assert
        Assert.Equal(0, handle);

        Assert.Equal(1, heap.Allocations.Length);
        Assert.Equal(8, heap.Pointer);

        Assert.Equal(0, allocation.Offset);
        Assert.Equal(8, allocation.Size);
        Assert.True(allocation.IsAllocated);
        Assert.False(allocation.IsMarked);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Heap_Allocate_Throws_ArgumentOutOfRangeException(int size)
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);

        // Act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => heap.Allocate(size, new Allocation()));
    }

    [Fact]
    public void Heap_GetEntry_Correctly_Returns_Bytes()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);
        var handle = heap.Allocate(8, new Allocation());

        // Act
        var bytes = heap.GetBytes(handle);

        // Assert
        Assert.Equal(8, bytes.Length);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Heap_GetEntry_Throws_ArgumentOutOfRangeException(int handle)
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);
        heap.Allocate(8, new Allocation());

        // Act & assert
        Assert.Throws<ArgumentOutOfRangeException>(() => heap.GetEntry(handle));
    }

    [Fact]
    public void Heap_Reclaim_Correctly_Reclaims_Unmarked_Allocations()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);
        var unmarked = heap.Allocate(8, new Allocation());
        ref var unmarkedAllocation = ref heap.GetEntry(unmarked);

        // Act
        heap.Reclaim();

        // Assert
        Assert.Equal(1, heap.Allocations.Length);

        Assert.False(unmarkedAllocation.IsAllocated);
        Assert.False(unmarkedAllocation.IsMarked);
    }

    [Fact]
    public void Heap_Reclaim_Correctly_Unmarks_Marked_Allocations()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);
        var marked = heap.Allocate(8, new Allocation());
        ref var markedAllocation = ref heap.GetEntry(marked);
        markedAllocation.IsMarked = true;

        // Act
        heap.Reclaim();

        // Assert
        Assert.Equal(1, heap.Allocations.Length);

        Assert.True(markedAllocation.IsAllocated);
        Assert.False(markedAllocation.IsMarked);
    }

    [Fact]
    public void Heap_Compress_Correctly_Compresses_Allocations()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);

        var alloc1 = heap.Allocate(8, new Allocation());
        ref var alloc1Ref = ref heap.GetEntry(alloc1);
        alloc1Ref.IsMarked = true;

        var alloc2 = heap.Allocate(8, new Allocation());
        ref var alloc2Ref = ref heap.GetEntry(alloc2);

        var alloc3 = heap.Allocate(8, new Allocation());
        ref var alloc3Ref = ref heap.GetEntry(alloc3);
        alloc3Ref.IsMarked = true;

        // Act
        heap.Reclaim();
        heap.Compress();

        // Assert
        Assert.Equal(0, alloc1Ref.Offset);
        Assert.Equal(8, alloc3Ref.Offset);
        Assert.Equal(16, heap.Pointer);
    }

    [Fact]
    public void Heap_GetCompressionMap_Correctly_Builds_CompressionMap()
    {
        // Arrange
        var heap = new Heap<Allocation>(new byte[512]);
        var alloc1 = heap.Allocate(8, new Allocation());
        ref var alloc1Ref = ref heap.GetEntry(alloc1);
        alloc1Ref.IsMarked = true;

        heap.Allocate(8, new Allocation());

        var alloc2 = heap.Allocate(8, new Allocation());
        ref var alloc2Ref = ref heap.GetEntry(alloc2);
        alloc2Ref.IsMarked = true;

        heap.Reclaim();
        heap.Compress();

        // Act
        var alloc3 = heap.Allocate(8, new Allocation());
        ref var alloc3Ref = ref heap.GetEntry(alloc3);
        alloc1Ref.IsMarked = true;
        alloc2Ref.IsMarked = true;
        alloc3Ref.IsMarked = true;
        heap.Reclaim();
        heap.Compress();

        // Assert
        Assert.Equal(0, alloc1Ref.Offset);
        Assert.Equal(8, alloc2Ref.Offset);
        Assert.Equal(16, alloc3Ref.Offset);
        Assert.Equal(24, heap.Pointer);
    }
}