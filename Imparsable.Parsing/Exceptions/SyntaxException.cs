using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing.Exceptions;

public class SyntaxException(ISourceMarker marker, string message) : MarkedException(marker, message);