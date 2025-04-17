using Domain;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class AttributeAdapter(ISimAttributeService simAttributeService, ISimClassService simClassService) : IAttributeAdapter
{
    private readonly ISimAttributeService _simAttributeService = simAttributeService;
    private readonly ISimClassService _simClassService = simClassService;

    public void DeleteAttribute(Guid attributeId)
    {
        try
        {
            _simAttributeService.DeleteAttribute(attributeId);
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public UpdatedAttributeResponse UpdateAttribute(Guid attributeId, AttributeRequest attribute)
    {
        var relatedClass = _simClassService.GetSimClassById(attribute.RelatedClassId);
        var type = _simClassService.GetSimClassById(attribute.TypeId);
        var updatedAttribute = new SimAttribute()
        {
            Id = attributeId,
            Name = attribute.Name,
            Privacity = EnumMapper.MapToDomainPrivacity(attribute.Privacity),
            RelatedClass = relatedClass,
            Type = type,
        };
        _simAttributeService.UpdateAttribute(attributeId, updatedAttribute);

        var response = new UpdatedAttributeResponse()
        {
            Attribute = new AttributeResponse()
            {
                Id = attributeId,
                Name = attribute.Name,
                Privacity = attribute.Privacity,
                RelatedClassId = attribute.RelatedClassId,
                TypeId = attribute.TypeId
            },
            Message = "Attribute updated successfully."
        };

        return response;
    }

    public CreatedAttributeResponse CreateAttribute(Guid id, AttributeRequest attribute)
    {
        throw new NotImplementedException();
    }
}
