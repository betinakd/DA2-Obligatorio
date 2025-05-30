using Adapter.Exceptions;
using Adapter.Helpers;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBusinessLogic;
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

    public UpdatedAttributeResponse UpdateAttribute(AttributeRequestUpdate attribute)
    {
        try
        {
            var relatedClass = _simClassService.GetSimClassById(attribute.RelatedClassId);
            var type = _simClassService.GetSimClassById(attribute.ReferenceId);
            var instance = _simClassService.GetSimClassById(attribute.InstanceId);

            _simClassService.ValidPolymorphism(type, instance);

            var updatedAttribute = new SimAttribute()
            {
                Id = attribute.IdAttribute,
                Name = attribute.Name,
                Privacity = EnumMapper.MapToDomainPrivacity(attribute.Privacity),
                RelatedClass = relatedClass,
                RelatedClassId = relatedClass.Id,
                Reference = type,
                ReferenceId = type.Id,
                Instance = instance,
                InstanceId = instance.Id,
                IsStatic = attribute.IsStatic
            };
            _simAttributeService.UpdateAttribute(attribute.IdAttribute, updatedAttribute);

            var response = new UpdatedAttributeResponse()
            {
                Attribute = AttributeResponseMapper.MapToAttributeResponse(updatedAttribute),
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
            var type = _simClassService.GetSimClassById(attribute.ReferenceId);
            var instance = _simClassService.GetSimClassById(attribute.InstanceId);

            _simClassService.ValidPolymorphism(type, instance);

            var newAttribute = new SimAttribute()
            {
                Id = Guid.NewGuid(),
                Name = attribute.Name,
                Privacity = EnumMapper.MapToDomainPrivacity(attribute.Privacity),
                RelatedClass = relatedClass,
                RelatedClassId = relatedClass.Id,
                Reference = type,
                ReferenceId = type.Id,
                Instance = instance,
                InstanceId = instance.Id,
                IsStatic = attribute.IsStatic
            };

            var createdAttribute = _simAttributeService.CreateAttribute(id, newAttribute);

            var response = new CreatedAttributeResponse()
            {
                Attribute = AttributeResponseMapper.MapToAttributeResponse(createdAttribute),
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

    public AttributeResponse GetAttribute(Guid id)
    {
        try
        {
            var attribute = _simAttributeService.GetSimAttribute(id);
            return AttributeResponseMapper.MapToAttributeResponse(attribute);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }
}
