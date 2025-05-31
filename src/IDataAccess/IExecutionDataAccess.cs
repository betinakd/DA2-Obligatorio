using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    SimMethod? FindMethodInHierarchy(SimClass objClass, Signature signature, int level = 0);
    public bool MethodIsInUseByInheritingInvocations(Guid methodId);

    public bool CanOverride(Guid idClass, SimMethod method);
    void SaveExecutionLog(ExecutionLog executionLog);
    public List<SimClass> GetFilteredClasses(Func<IQueryable<SimClass>, IQueryable<SimClass>> filter);
    public SimMethod FindMethodInHierarchyPublicOrProtected(SimClass simClass, Signature signature, int level = 0);
    public SimMethod FindSealedMethodInHierarchy(Guid classId, SimMethod methodToCheck);
    public SimMethod? FindOverrideOrReferenceMethod(SimClass instanceClass, SimClass referenceClass, Signature signature);
}
