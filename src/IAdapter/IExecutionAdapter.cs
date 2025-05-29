using Models.Request;
using Transformers.Abstractions;

namespace IAdapter;

public interface IExecutionAdapter
{
    string ExecuteMethod(MethodExecutionRequest request);
    TransformedResponse ExecuteMethodWithTransform(Guid apiKey, MethodExecutionRequest request, string transformerId = null);
    public bool IsAuthorizedUser(Guid apiKey);
}
