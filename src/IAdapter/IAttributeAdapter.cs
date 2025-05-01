using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IAttributeAdapter
{
    public void DeleteAttribute(Guid attributeId);
    public UpdatedAttributeResponse UpdateAttribute(Guid attributeId, AttributeRequest attribute);
    public CreatedAttributeResponse CreateAttribute(Guid id, AttributeRequest attribute);
    public AttributeResponse GetAttribute(Guid id);
}
