namespace Imparsable.Tools.Parsing.UnitTests;

public class SourceTests
{
    private const string HelloWorld = "Hello World";

    [Fact]
    public void Source_Advance_Correctly_Increments()
    {
        // Arrange
        var source = new Source(HelloWorld);

        // Act
        source.Advance();

        // Assert
        Assert.Equal('e', source.Current);
        Assert.Equal(2, source.Column);
    }

    [Fact]
    public void Source_Ended_Correctly_Detects()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.Length))
            source.Advance();

        // Act
        var ended = source.Ended();

        // Assert
        Assert.True(ended);
    }

    [Fact]
    public void Source_Extract_Returns_Correct_Range()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act
        var range = source.Extract();

        // Assert
        Assert.Equal(0, range.Offset);
        Assert.Equal(5, range.Length);
    }

    [Fact]
    public void Source_GetText_Returns_Correct_String()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act
        var range = source.Extract();
        var text = source.GetText(range.Offset, range.Length);

        // Assert
        var expected = HelloWorld.Substring(range.Offset, range.Length);
        Assert.Equal(expected, text);
    }

    [Fact]
    public void Source_Ignore_Correctly_Discards_Range()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act
        source.Ignore();

        // Assert
        Assert.Equal(' ', source.Current);
    }

    [Fact]
    public void Source_Check_Correctly_Handles_Matching_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.True(source.Check(' '));
    }

    [Fact]
    public void Source_Check_Correctly_Handles_Mismatched_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.False(source.Check('W'));
    }

    [Fact]
    public void Source_CheckAny_Correctly_Handles_Matching_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.True(source.CheckAny(['W', ' ']));
    }

    [Fact]
    public void Source_CheckAny_Correctly_Handles_Mismatched_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.False(source.CheckAny(['l', 'd']));
    }

    [Fact]
    public void Source_Match_Correctly_Handles_Matching_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.True(source.Match(' '));
        Assert.Equal(7, source.Column);
    }

    [Fact]
    public void Source_Match_Correctly_Handles_Mismatched_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.False(source.Match('W'));
        Assert.Equal(6, source.Column);
    }

    [Fact]
    public void Source_MatchAny_Correctly_Handles_Matching_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.True(source.MatchAny([' ', 'W']));
        Assert.Equal(7, source.Column);
    }

    [Fact]
    public void Source_MatchAny_Correctly_Handles_Mismatched_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.False(source.MatchAny(['o', 'r']));
        Assert.Equal(6, source.Column);
    }

    [Fact]
    public void Source_MatchSequence_Correctly_Handles_Matching_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.True(source.MatchSequence([' ', 'W']));
        Assert.Equal(8, source.Column);
    }

    [Fact]
    public void Source_MatchSequence_Correctly_Handles_Mismatched_Char()
    {
        // Arrange
        var source = new Source(HelloWorld);
        foreach (var _ in Enumerable.Range(0, HelloWorld.IndexOf(' ')))
            source.Advance();

        // Act & assert
        Assert.False(source.MatchSequence(['o', 'r']));
        Assert.Equal(6, source.Column);
    }
}