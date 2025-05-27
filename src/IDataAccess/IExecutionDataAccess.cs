using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    SimMethod? FindMethodInHierarchy(SimClass objClass, Signature signature, int level = 0);
    public bool MethodIsInUseByInheritingInvocations(Guid methodId);

    public bool MethodIsOverridingSealed(Guid idClass, SimMethod method);
    void SaveExecutionLog(ExecutionLog executionLog);
    public List<SimClass> GetFilteredClasses(Func<IQueryable<SimClass>, IQueryable<SimClass>> filter);
    public SimMethod FindMethodInHierarchyPublicOrProtected(SimClass simClass, Signature signature, int level = 0);
}
