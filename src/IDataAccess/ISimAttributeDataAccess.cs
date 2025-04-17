using Domain;

namespace IDataAccess;

public interface ISimAttributeDataAccess
{
    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute);
    public bool ExistAttributeName(Guid classId, string attributeName);
}
