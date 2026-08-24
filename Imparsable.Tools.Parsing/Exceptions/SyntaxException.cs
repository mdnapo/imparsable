using Imparsable.Tools.Parsing.Interfaces;

namespace Imparsable.Tools.Parsing.Exceptions;

public class SyntaxException(ISourceMarker marker, string message) : MarkedException(marker, message);