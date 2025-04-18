using Domain;

namespace IDataAccess;

public interface ISimMethodDataAccess
{
    public bool ExistMethodById(Guid id);
    public Invocation CreateInvocation(Guid idMethod, Invocation newInvocation);
    bool ExistsMethodInClass(Guid idClass, SimMethod method);
    SimMethod CreateMethod(Guid idClass, SimMethod method);
    bool ExistInvocationById(Guid id);
    Invocation GetInvocationById(Guid id);
    SimMethod GetMethodById(Guid id);
    bool ExistParameter(Guid id);
    Parameter GetParameterById(Guid id);
    bool ExistVariableById(Guid id);
    LocalVariable GetVariableById(Guid id);
    bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable);
    LocalVariable AddLocalVariable(Guid methodId, LocalVariable localVariable);
    bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter);
    Parameter AddMethodParameter(Guid methodId, Parameter parameter);
    void DeleteMethod(Guid id);
}
