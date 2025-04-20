using System.Diagnostics.CodeAnalysis;
using Domain;
using IDataAccess;

namespace DataAccess;
[ExcludeFromCodeCoverage]
public class ExecutionDataAccess : IExecutionDataAccess
{
    public bool ExecuteAbstractMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType)
    {
        throw new NotImplementedException();
    }

    public bool FoundPrivateMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType)
    {
        throw new NotImplementedException();
    }

    public bool FoundSealedMethod(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType)
    {
        throw new NotImplementedException();
    }

    public string GetExecution(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType)
    {
        throw new NotImplementedException();
    }

    public bool NotFoundMethodFirm(string methodName, List<Parameter> parameters, Guid idInstanceType, Guid idReferenceType)
    {
        throw new NotImplementedException();
    }
}
