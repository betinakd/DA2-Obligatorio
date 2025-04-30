using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class ExecutionService(IExecutionDataAccess executionDataAccess) : IExecutionService
{
    private readonly IExecutionDataAccess _executionDA = executionDataAccess;

    public string ExecuteMethod(Reference reference, Reference objReal, Signature signature, int level = 0, HashSet<Guid>? visited = null)
    {
        visited ??= [];
        var identation = new string(' ', level * 4);

        SimClass objClass = objReal.GetSimClass();
        SimMethod? methodToExecute = _executionDA.FindMethodInHierarchy(objClass, signature);

        if(methodToExecute == null)
        {
            return $"Error: No se encontró el método {signature.Name} en {objClass.Name}\n";
        }

        if(visited.Contains(methodToExecute.Id))
        {
            return $"{identation}{reference.GetSignature(signature)} -> {methodToExecute.GetMethodSignature(signature)} /* recursión */";
        }

        var result = $"{identation}{reference.GetSignature(signature)} -> {methodToExecute.GetMethodSignature(signature)}\n";

        if(level == 0)
        {
            result = $"{identation}{reference.GetSignatureWithClassName(signature)} -> {methodToExecute.GetMethodSignature(signature)}\n";
        }

        visited.Add(methodToExecute.Id);

        foreach(var invocation in methodToExecute.Invocations)
        {
            Reference invocRef = invocation.Reference;
            var isReferenceThis = invocation.Reference is ReferenceThis;
            Reference invocObj = isReferenceThis ? objReal : invocation.Reference;

            result += ExecuteMethod(invocRef, invocObj, invocation.Signature, level + 1, [.. visited]);
        }

        return result;
    }

    public void ValidateMethodExistsInClass(SimClass classId, Signature methodName)
    {
        if(_executionDA.FindMethodInHierarchy(classId, methodName) == null)
        {
            throw new NonExistentValueLogic($"Method '{methodName.Name}' is not accessible from this context");
        }
    }
}
