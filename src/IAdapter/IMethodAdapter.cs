using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IMethodAdapter
{
    public MethodResponse GetMethod(Guid id);
    public CreatedMethodResponse CreateMethod(Guid idClass, MethodRequestCreateClass method);
    public void DeleteMethod(Guid id);
    public VariableResponse GetVariable(Guid id);
    public CreatedVariableResponse CreateVariable(Guid idMethod, VariablesRequestCreateClass variable);
    public ParameterResponse GetParameter(Guid id);
    public CreatedParameterResponse CreateParameter(Guid idMethod, ParameterRequestCreateClass parameter);
    public CreatedInvocationResponse CreateInvocation(Guid idMethod, InvocationRequestCreateClass method);
    public InvocationResponse GetInvocation(Guid id);
}
