using System.Diagnostics.CodeAnalysis;
using Domain;
using IDataAccess;

namespace DataAccess;

[ExcludeFromCodeCoverage]
public class SimAttributeDataAccess : ISimAttributeDataAccess
{
    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute)
    {
        throw new NotImplementedException();
    }

    public void DeleteAttribute(Guid attributeId)
    {
        throw new NotImplementedException();
    }

    public bool ExistAttributeById(Guid attributeId)
    {
        throw new NotImplementedException();
    }

    public bool ExistAttributeName(Guid classId, string attributeName)
    {
        throw new NotImplementedException();
    }

    public bool InUseByOther(Guid attributeId)
    {
        throw new NotImplementedException();
    }

    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute)
    {
        throw new NotImplementedException();
    }
}
