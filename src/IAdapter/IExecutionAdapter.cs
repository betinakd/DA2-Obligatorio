using Models.Request;

namespace IAdapter;
public interface IExecutionAdapter
{
    string ExecuteMethod(MethodExecutionRequest request);
}
