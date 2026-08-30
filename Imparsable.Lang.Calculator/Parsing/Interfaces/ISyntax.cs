using Imparsable.Toolchain.Parsing.Interfaces;
using Imparsable.Toolchain.SourceGenerators.Attributes;

namespace Imparsable.Lang.Calculator.Parsing.Interfaces;

[VoidVisitorNode]
[TypedVisitorNode]
public partial interface ISyntax : ISyntax<Token>;