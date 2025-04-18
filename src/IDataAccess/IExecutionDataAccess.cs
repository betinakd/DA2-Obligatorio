using Domain;

namespace IDataAccess;

public interface IExecutionDataAccess
{
    bool ExecuteAbstractMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType);
    bool FoundPrivateMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType);
    bool FoundSealedMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType);
    bool NotFoundMethodFirm(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType);
}
