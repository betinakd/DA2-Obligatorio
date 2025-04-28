using Domain;

namespace IBussinesLogic;

public interface IExecutionService
{
    string ExecuteMethod(Reference reference, Reference objReal, Signature signature, int level = 0, HashSet<Guid>? visited = null);
    void ValidateMethodExistsInClass(SimClass classId, Signature methodName);
}
