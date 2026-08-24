using Imparsable.Tools.Parsing.Interfaces;
using Imparsable.Tools.Parsing.SourceGenerators.Attributes;

namespace Imparsable.Lang.Calculator.Parsing;

[VoidVisitorNode]
[TypedVisitorNode]
public partial interface ISyntax : ISyntax<Token>;