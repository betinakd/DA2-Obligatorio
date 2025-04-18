using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    bool ExecuteAbstractMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType);
}
