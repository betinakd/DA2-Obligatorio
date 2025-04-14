using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IAttributeAdapter
{
    public DeletedAttributeResponse DeleteAttribute(Guid attributeId);
    public UpdatedAttributeResponse UpdateAttribute(Guid attributeId, AttributeRequest attribute);
}
