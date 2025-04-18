using Domain;

namespace IDataAccess;

public interface ISimMethodDataAccess
{
    public bool ExistMethodById(Guid id);
    public Invocation CreateInvocation(Guid idMethod, Invocation newInvocation);
    bool ExistsMethodInClass(Guid idClass, SimMethod method);
    SimMethod CreateMethod(Guid idClass, SimMethod method);
    bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable);
    LocalVariable AddLocalVariable(Guid methodId, LocalVariable localVariable);
}
