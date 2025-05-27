using Domain;

namespace IBusinessLogic;

public interface IExecutionService
{
    string ExecuteMethod(Reference reference, Reference objReal, Signature signature, int level = 0, HashSet<Guid>? visited = null);
    void ValidateMethodExistsInClass(SimClass classId, Signature methodName, bool isNotAbstract);
    public void MethodIsOverridingSealed(Guid idClass, SimMethod method);
    public void SaveExecutionLog(string reference, string objCreate, string execution);
    bool IsReferenceBaseOfInstance(SimClass refer, SimClass obj);
}
