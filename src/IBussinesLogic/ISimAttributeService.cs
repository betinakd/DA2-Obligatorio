using Domain;

namespace IBussinesLogic;

public interface ISimAttributeService
{
    public void DeleteAttribute(Guid attributeId);
    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute);
    public void CreateAttribute(Guid id, SimAttribute attribute);
}
