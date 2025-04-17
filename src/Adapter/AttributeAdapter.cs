using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class AttributeAdapter(ISimAttributeService simAttributeService) : IAttributeAdapter
{
    private readonly ISimAttributeService _simAttributeService = simAttributeService;

    public void DeleteAttribute(Guid attributeId)
    {
        _simAttributeService.DeleteAttribute(attributeId);
    }

    public UpdatedAttributeResponse UpdateAttribute(Guid attributeId, AttributeRequest attribute)
    {
        throw new NotImplementedException();
    }

    public CreatedAttributeResponse CreateAttribute(Guid id, AttributeRequest attribute)
    {
        throw new NotImplementedException();
    }
}