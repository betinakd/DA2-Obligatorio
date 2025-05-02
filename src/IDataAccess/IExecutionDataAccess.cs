using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    SimMethod? FindMethodInHierarchy(SimClass objClass, Signature signature, int level = 0);
    public bool MethodIsInUseByInheritingInvocations(Guid methodId);

    public bool ClassInheritAttribute(Guid classId, Guid attributeId, int level = 0);
    public bool MethodIsOverridingSealed(Guid idClass, SimMethod method);
    void SaveExecutionLog(ExecutionLog executionLog);
}
