using Domain;

namespace IDataAccess;

public interface ISimAttributeDataAccess
{
    public SimAttribute CreateAttribute(Guid claseId, SimAttribute attribute);
}
