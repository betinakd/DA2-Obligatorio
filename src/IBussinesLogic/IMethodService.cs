using Domain;

namespace IBussinesLogic;

public interface IMethodService
{
    public SimMethod GetMethodById(Guid id);
    public Parameter AddMethodParameter(Guid methodId, Parameter parameter);
    public LocalVariable AddLocalVariable(Guid methodId, LocalVariable localVariable);
    public Parameter GetParameterById(Guid id);
    public LocalVariable GetVariableById(Guid id);
    public SimMethod AddMethod(Guid id, SimMethod method);
    public void DeleteMethod(Guid id);
    public Invocation AddInvocation(Guid idMethod, Invocation newInvocation);
    public Invocation GetInvocationById(Guid id);
}
