using Imparsable.Parsing;
using Imparsable.Parsing.SourceGenerators.Attributes;

namespace Imparsable.Tool.Calculator.Syntax;

[VoidVisitorNode]
[TypedVisitorNode]
public partial interface ISyntax : ISyntax<Token>;
