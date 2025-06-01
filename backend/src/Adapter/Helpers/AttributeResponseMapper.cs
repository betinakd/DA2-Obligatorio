using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;

[ExcludeFromCodeCoverage]
public class AttributeResponseMapper
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
            ReferenceId = domainAttribute.ReferenceId,
            InstanceId = domainAttribute.InstanceId,
            Privacity = EnumMapper.MapToModelPrivacity(domainAttribute.Privacity),
            RelatedClassId = domainAttribute.RelatedClassId,
            IsStatic = domainAttribute.IsStatic
        };
    }
}
