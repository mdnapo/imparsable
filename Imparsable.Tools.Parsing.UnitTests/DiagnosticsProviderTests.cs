using Imparsable.Tools.Parsing.Interfaces;

namespace Imparsable.Tools.Parsing.UnitTests;

public class DiagnosticsProviderTests
{
    private const string Message = "This is just a test.";
    
    private readonly struct SourceMarker(int offset, int length, int line, int column) : ISourceMarker
    {
        public int Offset { get; } = offset;
        public int Length { get; } = length;
        public int Line { get; } = line;
        public int Column { get; } = column;
    }

    [Fact]
    public void DiagnosticsProvider_Warning_Correctly_Adds_Warning()
    {
        // Arrange
        var list = new List<Diagnostic>();
        var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += list.Add;
        var marker = new SourceMarker(0, 0, 1, 1);

        // Act
        diagnostics.Warning(marker, Message);

        // Assert
        Assert.Contains(list, d => 
            d.Severity == DiagnosticSeverity.WARNING && 
            d.Marker.Equals(marker) && 
            d.Message == Message
        );
        Assert.Contains(diagnostics, d => 
            d.Severity == DiagnosticSeverity.WARNING && 
            d.Marker.Equals(marker) && 
            d.Message == Message
        );
    }

    [Fact]
    public void DiagnosticsProvider_Error_Correctly_Adds_Error()
    {
        // Arrange
        var list = new List<Diagnostic>();
        var diagnostics = new DiagnosticsProvider();
        diagnostics.Published += list.Add;
        var marker = new SourceMarker(0, 0, 1, 1);

        // Act
        diagnostics.Error(marker, Message);

        // Assert
        Assert.Contains(list, d => 
            d.Severity == DiagnosticSeverity.ERROR && 
            d.Marker.Equals(marker) && 
            d.Message == Message
        );
        Assert.Contains(diagnostics, d => 
            d.Severity == DiagnosticSeverity.ERROR && 
            d.Marker.Equals(marker) && 
            d.Message == Message
        );
    }
}