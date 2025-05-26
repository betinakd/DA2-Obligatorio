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
            ReturnTypeId = domainMethod.ReturnTypeId,
            Privacity = EnumMapper.MapToModelPrivacity(domainMethod.Privacity),
            Accesibility = EnumMapper.MapToModelAccesibility(domainMethod.Accesibility),
            Parameters = domainMethod.Parameters.Select(p => new ParameterResponse
            {
                Id = p.Id,
                Name = p.Name,
                MethodId = p.RelatedMethodId,
                ClassTypeId = p.TypeId
            }).ToList(),
            Variables = domainMethod.LocalVariables?.Select(v => new VariableResponse
            {
                Id = v.Id,
                Name = v.Name,
                MethodId = v.RelatedMethodId,
                ClassTypeId = v.TypeId
            }).ToList() ?? [],
            Invocations = domainMethod.Invocations?.Select(i => new InvocationResponse
            {
                Id = i.Id,
                IdReference = i.Reference.GetReferenceId(),
                MethodName = i.Signature.Name,
                Parameters = i.Signature.Parameters?.Select(p => new ParameterResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ClassTypeId = p.TypeId
                }).ToList() ?? []
            }).ToList() ?? []
        };
    }
}
