using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;

[ExcludeFromCodeCoverage]
public static class SimClassResponseMapper
{
    public static SimClassResponse MapToSimClassResponse(SimClass domainClass)
    {
        return new SimClassResponse()
        {
            Id = domainClass.Id,
            Name = domainClass.Name,
            State = EnumMapper.MapToModelAccesibility(domainClass.State),
            IdBaseClass = domainClass.BaseClassId,
            Methods = domainClass.Methods.Select(m => new MethodResponse
            {
                Id = m.Id,
                IdClassOwner = m.RelatedClassId,
                Name = m.Name,
                ReturnTypeId = m.ReturnTypeId,
                Privacity = EnumMapper.MapToModelPrivacity(m.Privacity),
                Accesibility = EnumMapper.MapToModelAccesibility(m.Accesibility),
                Parameters = m.Parameters.Select(p => new ParameterResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ClassTypeId = p.TypeId,
                    MethodId = p.RelatedMethodId
                }).ToList(),
                Variables = m.LocalVariables.Select(v => new VariableResponse
                {
                    Id = v.Id,
                    Name = v.Name,
                    ClassTypeId = v.TypeId,
                    MethodId = v.RelatedMethodId
                }).ToList(),
                Invocations = m.Invocations.Select(i => new InvocationResponse
                {
                    Id = i.Id,
                    IdReference = i.Reference?.GetReferenceId() ?? i.ReferenceId ?? Guid.Empty,
                    TypeReference = i.Reference?.GetReferenceTypeDescription(),
                    MethodName = i.Signature?.Name,
                    Parameters = i.Signature?.Parameters.Select(p => new ParameterResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ClassTypeId = p.TypeId
                    }).ToList() ?? []
                }).ToList()
            }).ToList(),
            Attributes = domainClass.Attributes.Select(a => new AttributeResponse
            {
                Id = a.Id,
                Name = a.Name,
                TypeId = a.TypeId,
                Privacity = EnumMapper.MapToModelPrivacity(a.Privacity),
                RelatedClassId = a.RelatedClassId
            }).ToList()
        };
    }
}
