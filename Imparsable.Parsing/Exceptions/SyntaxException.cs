namespace Imparsable.Parsing.Exceptions;

public class SyntaxException(string message) : MarkedException<SyntaxException>(message);