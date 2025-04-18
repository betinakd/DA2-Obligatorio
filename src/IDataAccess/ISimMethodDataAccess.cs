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
}
