using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;

[ExcludeFromCodeCoverage]
public class SimClassResponseMapper
{
    public static SimClassResponse MapToSimClassResponse(SimClass domainClass)
    {
        return new SimClassResponse()
        {
            Id = domainClass.Id,
            Name = domainClass.Name,
            BaseNamespace = new NamespaceResponse
            {
                Id = domainClass.Namespace.Id,
                Name = domainClass.Namespace.Name,
            },
            State = EnumMapper.MapToModelAccesibility(domainClass.State),
            IdBaseClass = domainClass.BaseClassId,
            Methods = domainClass.Methods.Select(m => new MethodResponse
            {
                Id = m.Id,
                IdClassOwner = m.RelatedClassId,
                Name = m.Name,
                ReturnTypeId = m.ReturnTypeId,
                IsStatic = m.IsStatic,
                Privacity = EnumMapper.MapToModelPrivacity(m.Privacity),
                Accesibility = EnumMapper.MapToModelAccesibility(m.Accesibility),
                Parameters = m.Parameters.Select(p => new ParameterResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ReferenceId = p.ReferenceId,
                    MethodId = p.RelatedMethodId
                }).ToList(),
                Variables = m.LocalVariables.Select(v => new VariableResponse
                {
                    Id = v.Id,
                    Name = v.Name,
                    ReferenceId = v.ReferenceId,
                    InstanceId = v.InstanceId,
                    MethodId = v.RelatedMethodId
                }).ToList(),
                Invocations = m.Invocations.Select(i => new InvocationResponse
                {
                    Id = i.Id,
                    IdReference = i.Reference?.GetReferenceId() ?? i.ReferenceId ?? Guid.Empty,
                    TypeReference = i.Reference?.GetReferenceTypeDescription(),
                    MethodName = i.Signature?.Name,
                    Parameters = i.Signature?.Parameters.Select(p => new ParameterSignatureResponse
                    {
                        Name = p.Name,
                        ReferenceId = p.ReferenceId,
                        InstanceId = p.InstanceId
                    }).ToList() ?? []
                }).ToList()
            }).ToList(),
            Attributes = domainClass.Attributes.Select(a => new AttributeResponse
            {
                Id = a.Id,
                Name = a.Name,
                ReferenceId = a.ReferenceId,
                InstanceId = a.InstanceId,
                Privacity = EnumMapper.MapToModelPrivacity(a.Privacity),
                RelatedClassId = a.RelatedClassId,
                IsStatic = a.IsStatic
            }).ToList(),
            Implements = domainClass.Implements.Select(i => new InterfaceResponse
            {
                Id = i.Id,
                Name = i.Name,
                Methods = i.Methods.Select(m => new MethodResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    IdClassOwner = m.RelatedClassId,
                    ReturnTypeId = m.ReturnTypeId,
                    Privacity = EnumMapper.MapToModelPrivacity(m.Privacity),
                    Accesibility = EnumMapper.MapToModelAccesibility(m.Accesibility),
                    Parameters = m.Parameters.Select(p => new ParameterResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ReferenceId = p.ReferenceId,
                        MethodId = p.RelatedMethodId
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }
}
