using StreamJsonRpc;

namespace Imparsable.LSP.Protocol;

[AttributeUsage(AttributeTargets.Method)]
public sealed class LspMethodAttribute : JsonRpcMethodAttribute
{
    public LspMethodAttribute(string name) : base(name)
    {
        UseSingleObjectParameterDeserialization = true;
    }
}