using Models.Request;
using Transformers.Abstractions;

namespace IAdapter;

public interface IExecutionAdapter
{
    string ExecuteMethod(MethodExecutionRequest request);
    TransformedResponse ExecuteMethodWithTransform(MethodExecutionRequest request, string transformerId = null);
}
