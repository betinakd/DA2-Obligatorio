using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class ExecutionService(ISimClassDataAccess simClassDA) : IExecutionService
{
    private readonly ISimClassDataAccess _simClassDA = simClassDA;

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

        return null;
    }
}
