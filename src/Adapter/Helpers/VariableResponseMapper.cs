using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;

[ExcludeFromCodeCoverage]
public class VariableResponseMapper
{
    public static VariableResponse MapToVariableResponse(LocalVariable domainVariable)
    {
        if(domainVariable == null)
        {
            return null;
        }

        return new VariableResponse()
        {
            Id = domainVariable.Id,
            Name = domainVariable.Name,
            ReferenceId = domainVariable.ReferenceId,
            InstanceId = domainVariable.InstanceId,
            MethodId = domainVariable.RelatedMethodId
        };
    }
}
