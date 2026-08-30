using Imparsable.Toolchain.Parsing.Interfaces;
using Imparsable.Toolchain.SourceGenerators.Attributes;

namespace Imparsable.Lang.Calculator.Parsing;

[VoidVisitorNode]
[TypedVisitorNode]
public partial interface ISyntax : ISyntax<Token>;