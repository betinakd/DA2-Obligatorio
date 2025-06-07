using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IExecutionAdapter
{
    MethodExecutionResponse ExecuteMethod(MethodExecutionRequest request);
    public bool IsAuthorizedUser(Guid apiKey);
}
