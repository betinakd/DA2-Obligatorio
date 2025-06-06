using Models.Request;
using Models.Response;
using Transformers.Abstractions;

namespace IAdapter;

public interface IExecutionAdapter
{
    MethodExecutionResponse ExecuteMethod(MethodExecutionRequest request);
    TransformedResponse ExecuteMethodWithTransform(MethodExecutionRequest request, string transformerId);
    public bool IsAuthorizedUser(Guid apiKey);
}
