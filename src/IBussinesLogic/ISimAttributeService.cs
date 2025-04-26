using Domain;

namespace IBussinesLogic;

public interface ISimAttributeService
{
    public void DeleteAttribute(Guid attributeId);
    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute);
    public SimAttribute CreateAttribute(Guid methodId, SimAttribute attribute);
    public SimAttribute GetSimAttribute(Guid attributeId);
}
