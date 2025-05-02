using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;
[ExcludeFromCodeCoverage]
public static class ParameterResponseMapper
{
    public static ParameterResponse MapToParameterResponse(Parameter domainParameter)
    {
        if(domainParameter == null)
        {
            return null;
        }

        return new ParameterResponse()
        {
            Id = domainParameter.Id,
            Name = domainParameter.Name,
            ClassTypeId = domainParameter.TypeId,
            MethodId = domainParameter.RelatedMethodId
        };
    }
}