using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;
[ExcludeFromCodeCoverage]
public static class VariableResponseMapper
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
            ClassTypeId = domainVariable.TypeId,
            MethodId = domainVariable.RelatedMethodId
        };
    }
}