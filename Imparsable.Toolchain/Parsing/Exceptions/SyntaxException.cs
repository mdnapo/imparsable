using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing.Exceptions;

public class SyntaxException(ISourceMarker marker, string message) : MarkedException(marker, message);