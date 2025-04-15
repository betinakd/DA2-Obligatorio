using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IMethodAdapter
{
    public MethodResponse GetMethod(Guid id);
    public CreatedMethodResponse CreateMethod(Guid idClass, MethodRequest method);
    public void DeleteMethod(Guid id);
    public VariableResponse GetVariable(Guid id);
    public CreatedVariableResponse CreateVariable(Guid idMethod, VariableRequest variable);
}
