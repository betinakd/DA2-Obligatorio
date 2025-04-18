using Domain;

namespace IDataAccess;

public interface ISimMethodDataAccess
{
    public bool ExistMethodById(Guid id);
    public Invocation CreateInvocation(Guid idMethod, Invocation newInvocation);
    bool ExistsMethodInClass(Guid idClass, SimMethod method);
}
