using Models.Response;

namespace IAdapter;

public interface IAttributeAdapter
{
    public AttributeResponse DeleteAttribute(Guid attributeId);
}