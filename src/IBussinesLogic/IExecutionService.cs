using Domain;

namespace IBussinesLogic;

public interface IExecutionService
{
    string ExecuteMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType, string instanceName);
}
