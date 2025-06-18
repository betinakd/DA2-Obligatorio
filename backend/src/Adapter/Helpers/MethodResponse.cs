using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;

[ExcludeFromCodeCoverage]
public class MethodResponseMapper
{
    public static MethodResponse MapToMethodResponse(SimMethod domainMethod)
    {
        if(domainMethod == null)
        {
            return null;
        }

        return new MethodResponse()
        {
            Id = domainMethod.Id,
            Name = domainMethod.Name,
            IdClassOwner = domainMethod.RelatedClassId,
            IsStatic = domainMethod.IsStatic,
            ReturnTypeId = domainMethod.ReturnTypeId,
            Privacity = EnumMapper.MapToModelPrivacity(domainMethod.Privacity),
            Accesibility = EnumMapper.MapToModelAccesibility(domainMethod.Accesibility),
            IsVirtual = domainMethod.IsVirtual,
            IsOverride = domainMethod.IsOverride,
            Parameters = domainMethod.Parameters.Select(p => new ParameterResponse
            {
                Id = p.Id,
                Name = p.Name,
                MethodId = p.RelatedMethodId,
                ReferenceId = p.ReferenceId
            }).ToList(),
            Variables = domainMethod.LocalVariables?.Select(v => new VariableResponse
            {
                Id = v.Id,
                Name = v.Name,
                MethodId = v.RelatedMethodId,
                ReferenceId = v.ReferenceId
            }).ToList() ?? [],
            Invocations = domainMethod.Invocations?.Select(i => new InvocationResponse
            {
                Id = i.Id,
                IdReference = i.Reference.GetReferenceId(),
                MethodName = i.Signature.Name,
                IdReturnType = i.Signature?.ReturnType?.Id ?? Guid.Empty,
                Parameters = i.Signature.Parameters?.Select(p => new ParameterSignatureResponse
                {
                    Name = p.Name,
                    ReferenceId = p.ReferenceId,
                    InstanceId = p.InstanceId
                }).ToList() ?? []
            }).ToList() ?? []
        };
    }
}
