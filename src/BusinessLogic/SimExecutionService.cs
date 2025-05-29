using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IBusinessLogic;
using IDataAccess;

namespace BusinessLogic;

public class ExecutionService(IExecutionDataAccess executionDataAccess, IApikeyDataAccess apikeyDataAccess) : IExecutionService
{
    private readonly IExecutionDataAccess _executionDA = executionDataAccess;
    private readonly IApikeyDataAccess _apikeyDA = apikeyDataAccess;

    public string ExecuteMethod(Reference reference, Reference objReal, Signature signature, int level = 0, HashSet<Guid>? visited = null)
    {
        visited ??= [];

        SimClass referenceClass = reference.GetSimClass();
        SimMethod? staticMethod = _executionDA.FindMethodInHierarchy(referenceClass, signature);

        if(staticMethod == null)
        {
            throw new InvalidOperationLogic($"Method not executable from reference.");
        }

        var useDynamicDispatch = staticMethod.Accesibility == SimAccesibility.Abstract || staticMethod.IsVirtual;

        return ExecuteMethodInternal(reference, objReal, signature, level, visited, useDynamicDispatch);
    }

    private string ExecuteMethodInternal(Reference reference, Reference objReal, Signature signature, int level, HashSet<Guid> visited, bool useDynamicDispatch)
    {
        var identation = new string(' ', level * 4);

        SimMethod? methodToExecute;

        if(useDynamicDispatch)
        {
            SimClass objClass = objReal.GetSimClass();
            methodToExecute = _executionDA.FindMethodInHierarchy(objClass, signature);
        }
        else
        {
            SimClass referenceClass = reference.GetSimClass();
            methodToExecute = _executionDA.FindMethodInHierarchy(referenceClass, signature);

            SimClass objClass = objReal.GetSimClass();
            if(methodToExecute != null &&
               methodToExecute.Privacity == SimPrivacity.Private &&
               referenceClass.Id != objClass.Id)
            {
                throw new InvalidOperationLogic($"Private method '{signature.Name}' not accessible from this context.");
            }
        }

        if(methodToExecute == null)
        {
            throw new InvalidOperationLogic($"Method not executable from reference.");
        }

        if(visited.Contains(methodToExecute.Id))
        {
            return $"{identation}{reference.GetSignature(signature)} -> {methodToExecute.GetMethodSignature(signature)}\n";
        }

        var result = level == 0
            ? $"{identation}{reference.GetSignatureWithClassName(signature)} -> {methodToExecute.GetMethodSignature(signature)}\n"
            : $"{identation}{reference.GetSignature(signature)} -> {methodToExecute.GetMethodSignature(signature)}\n";

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

    public void ValidateMethodExistsInClass(SimClass classId, Signature methodName, bool isNotAbstract)
    {
        var method = _executionDA.FindMethodInHierarchyPublicOrProtected(classId, methodName);
        if(method == null)
        {
            throw new NonExistentValueLogic($"Method '{methodName.Name}' is not accessible from this context");
        }

        if(method.Accesibility == SimAccesibility.Abstract && isNotAbstract)
        {
            throw new InvalidAttributeLogic($"Cannot add an abstract method to execute Method {methodName.Name}");
        }
    }

    public void SaveExecutionLog(string reference, string objCreate, string execution)
    {
        var executionLog = new ExecutionLog()
        {
            Execution = execution,
            ObjectCreate = objCreate,
            Reference = reference
        };
        _executionDA.SaveExecutionLog(executionLog);
    }

    public bool IsReferenceBaseOfInstance(SimClass refer, SimClass obj)
    {
        if(refer == null || obj == null)
        {
            return false;
        }

        if(refer.Id == obj.Id)
        {
            return true;
        }

        if(!obj.BaseClassId.HasValue)
        {
            return false;
        }

        if(obj.BaseClassId.Value == refer.Id)
        {
            return true;
        }

        var baseClass = _executionDA.GetFilteredClasses(query =>
            query.Where(c => c.Id == obj.BaseClassId.Value))
            .FirstOrDefault();

        if(baseClass == null)
        {
            return false;
        }

        return IsReferenceBaseOfInstance(refer, baseClass);
    }

    public bool IsAuthorizedUser(Guid apiKey)
    {
        var keyExists = _apikeyDA.ApiKeyExists(apiKey);
        return keyExists && !(apiKey == Guid.Empty);
    }
}
