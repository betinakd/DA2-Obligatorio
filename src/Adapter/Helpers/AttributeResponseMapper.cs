using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;
[ExcludeFromCodeCoverage]
public static class AttributeResponseMapper
{
    public static AttributeResponse MapToAttributeResponse(SimAttribute domainAttribute)
    {
        if(domainAttribute == null)
        {
            return null;
        }

        return new AttributeResponse()
        {
            Id = domainAttribute.Id,
            Name = domainAttribute.Name,
            TypeId = domainAttribute.TypeId,
            Privacity = EnumMapper.MapToModelPrivacity(domainAttribute.Privacity)
        };
    }
}
