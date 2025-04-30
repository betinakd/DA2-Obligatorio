using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    SimMethod? FindMethodInHierarchy(SimClass objClass, Signature signature);
    public bool MethodIsInUseByInheriting(Guid methodId);

    public bool ClassInheritAttribute(Guid classId, Guid attributeId, int level = 0);
}
