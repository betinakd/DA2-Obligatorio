using Domain;

namespace IBusinessLogic;

public interface IExecutionService
{
    public string ExecuteMethod(SimClass referenceClass, SimClass instanceClass, Reference reference, Signature signature, HashSet<Guid>? visited = null, int level = 0);
    void ValidateMethodExistsInClass(SimClass classId, Signature methodName, bool isNotAbstract);
    public void SaveExecutionLog(string reference, string objCreate, string execution);
    bool IsReferenceBaseOfInstance(SimClass refer, SimClass obj);
    public bool IsAuthorizedUser(Guid apiKey);
}
