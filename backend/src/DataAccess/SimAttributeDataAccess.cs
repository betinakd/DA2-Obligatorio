using DataAccess.Context;
using Domain;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

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
        var attribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);
        _context.SimAttributes.Remove(attribute!);
        _context.SaveChanges();
    }

    public bool InUseByOther(Guid attributeId)
    {
        return _context.References
            .OfType<ReferenceAttribute>()
            .Any(r => r.Reference.Id == attributeId);
    }

    public bool ExistAttributeById(Guid attributeId)
    {
        return _context.SimAttributes
            .Any(a => a.Id == attributeId);
    }

    public bool ExistAttributeName(Guid classId, string attributeName)
    {
        return _context.SimAttributes
            .Any(a => a.RelatedClassId == classId && a.Name.ToLower() == attributeName.ToLower());
    }

    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute)
    {
        DeleteAttribute(attributeId);
        _context.SimAttributes.Add(attribute);
        _context.SaveChanges();

        return attribute;
    }

    public SimAttribute GetSimAttribute(Guid attributeId)
    {
        var attribute = _context.SimAttributes
            .Include(a => a.Reference)
            .FirstOrDefault(a => a.Id == attributeId);
        return attribute;
    }
}
