using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class ExecutionService(IExecutionDataAccess executionDataAccess) : IExecutionService
{
    private readonly IExecutionDataAccess _executionDA = executionDataAccess;

    public string ExecuteMethod(Reference reference, Reference objReal, Signature signature, int level = 0, HashSet<Guid>? visited = null)
    {
        var identation = new string(' ', level * 4);

        SimClass objClass = objReal.GetSimClass();
        SimMethod? methodToExecute = _executionDA.FindMethodInHierarchy(objClass, signature);

        var result = $"{identation}{reference.GetSignature(signature)} -> {methodToExecute.RelatedClass.Name}.{methodToExecute.Name}()";

        return result;
    }
}
