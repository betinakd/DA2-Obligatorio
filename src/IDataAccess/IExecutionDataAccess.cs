using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    SimMethod? FindMethodInHierarchy(SimClass objClass, Signature signature, int level = 0);
    public bool MethodIsInUseByInheriting(Guid methodId);

    public bool ClassInheritAttribute(Guid classId, Guid attributeId, int level = 0);
}
