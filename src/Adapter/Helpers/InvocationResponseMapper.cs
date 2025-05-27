using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;
[ExcludeFromCodeCoverage]
public class InvocationResponseMapper
{
    public static InvocationResponse MapToInvocationResponse(Invocation domainInvocation)
    {
        if(domainInvocation == null)
        {
            return null;
        }

        return new InvocationResponse()
        {
            Id = domainInvocation.Id,
            IdReference = domainInvocation.Reference?.GetReferenceId() ?? domainInvocation.ReferenceId ?? Guid.Empty,
            TypeReference = domainInvocation.Reference?.GetReferenceTypeDescription(),
            MethodName = domainInvocation.Signature?.Name,
            Parameters = domainInvocation.Signature?.Parameters
                .Select(p => new ParameterResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    MethodId = domainInvocation.RelatedMethodId,
                    ClassTypeId = p.TypeId
                }).ToList() ?? []
        };
    }
}
