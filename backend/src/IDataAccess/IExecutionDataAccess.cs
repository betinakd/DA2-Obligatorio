using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    public bool MethodIsInUseByInheritingInvocations(Guid methodId);
    public bool CanOverrideFromBaseClass(Guid baseClassId, SimMethod methodToOverride);
    public SimMethod FindSealedMethodInHierarchyFromBaseClass(Guid baseClassId, SimMethod methodToCheck);

    void SaveExecutionLog(ExecutionLog executionLog);
    public List<SimClass> GetFilteredClasses(Func<IQueryable<SimClass>, IQueryable<SimClass>> filter);
    public SimMethod FindMethodInHierarchyPublicOrProtected(SimClass simClass, Signature signature, int level = 0);
    public SimMethod? FindOverrideOrReferenceMethod(SimClass instanceClass, SimClass referenceClass, Signature signature);
}
