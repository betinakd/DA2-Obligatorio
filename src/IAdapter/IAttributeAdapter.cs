using Models.Response;

namespace IAdapter;

public interface IAttributeAdapter
{
    public DeletedAttributeResponse DeleteAttribute(Guid attributeId);
}