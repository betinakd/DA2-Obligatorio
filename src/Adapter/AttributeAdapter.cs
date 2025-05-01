using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Exceptions;
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
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public UpdatedAttributeResponse UpdateAttribute(Guid attributeId, AttributeRequest attribute)
    {
        try
        {
            var relatedClass = _simClassService.GetSimClassById(attribute.RelatedClassId);
            var type = _simClassService.GetSimClassById(attribute.TypeId);
            var updatedAttribute = new SimAttribute()
            {
                Id = attributeId,
                Name = attribute.Name,
                Privacity = EnumMapper.MapToDomainPrivacity(attribute.Privacity),
                RelatedClass = relatedClass,
                RelatedClassId = relatedClass.Id,
                Type = type,
                TypeId = type.Id
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
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
    }

    public CreatedAttributeResponse CreateAttribute(Guid id, AttributeRequest attribute)
    {
        try
        {
            var relatedClass = _simClassService.GetSimClassById(id);
            var type = _simClassService.GetSimClassById(attribute.TypeId);

            var newAttribute = new SimAttribute()
            {
                Id = Guid.NewGuid(),
                Name = attribute.Name,
                Privacity = EnumMapper.MapToDomainPrivacity(attribute.Privacity),
                RelatedClass = relatedClass,
                RelatedClassId = relatedClass.Id,
                Type = type,
                TypeId = type.Id,
            };

            var createdAttribute = _simAttributeService.CreateAttribute(id, newAttribute);

            var response = new CreatedAttributeResponse()
            {
                Attribute = new AttributeResponse()
                {
                    Id = createdAttribute.Id,
                    Name = createdAttribute.Name,
                    Privacity = EnumMapper.MapToModelPrivacity(createdAttribute.Privacity),
                    RelatedClassId = createdAttribute.RelatedClass.Id,
                    TypeId = createdAttribute.Type.Id
                },
                Message = "Attribute created successfully."
            };

            return response;
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
    }
}
