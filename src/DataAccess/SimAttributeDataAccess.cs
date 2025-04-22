using System.Diagnostics.CodeAnalysis;
using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;

public class SimAttributeDataAccess(SimulatorDbContext context) : ISimAttributeDataAccess
{
    private readonly SimulatorDbContext _context = context;
    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute)
    {
        attribute.RelatedClassId = classId;
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        return attribute;
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
