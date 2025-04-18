using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class ExecutionService(ISimClassDataAccess simClassDA, IExecutionDataAccess executionDataAccess) : IExecutionService
{
    private readonly ISimClassDataAccess _simClassDA = simClassDA;
    private readonly IExecutionDataAccess _executionDA = executionDataAccess;

    public string ExecuteMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType, string instanceName)
    {
        if(!_simClassDA.ExistSimClassById(idInstanceType))
        {
            throw new NonExistentValueLogic("Instance class not found.");
        }

        if(!_simClassDA.ExistSimClassById(idReferenceType))
        {
            throw new NonExistentValueLogic("Reference class not found.");
        }

        if(_executionDA.ExecuteAbstractMethod(methodName, parameters, idInstanceType, idReferenceType))
        {
            throw new InvalidOperationLogic($"Can not execute {methodName} because it is abstract.");
        }

        if(_executionDA.FoundSealedMethod(methodName, parameters, idInstanceType, idReferenceType))
        {
            throw new InvalidOperationLogic($"Can not execute {methodName} because it is sealed for the instance.");
        }

        return null;
    }
}
