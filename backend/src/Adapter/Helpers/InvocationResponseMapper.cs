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
            IdReturnType = domainInvocation.Signature?.ReturnType?.Id ?? Guid.Empty,
            Parameters = domainInvocation.Signature?.Parameters
                .Select(p => new ParameterSignatureResponse
                {
                    Name = p.Name,
                    ReferenceId = p.ReferenceId,
                    InstanceId = p.InstanceId,
                }).ToList() ?? []
        };
    }
}
