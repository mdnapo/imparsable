using StreamJsonRpc;

namespace Imparsable.Toolchain.LSP.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class LspMethodAttribute : JsonRpcMethodAttribute
{
    public LspMethodAttribute(string name) : base(name)
    {
        UseSingleObjectParameterDeserialization = true;
    }
}