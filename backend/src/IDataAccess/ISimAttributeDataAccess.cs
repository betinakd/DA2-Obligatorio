using Domain;

namespace IDataAccess;

public interface ISimAttributeDataAccess
{
    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute);
    public bool ExistAttributeName(Guid classId, string attributeName);
    public bool ExistAttributeById(Guid attributeId);
    public void DeleteAttribute(Guid attributeId);
    public bool InUseByOther(Guid attributeId);
    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute);
    SimAttribute GetSimAttribute(Guid attributeId);
}
