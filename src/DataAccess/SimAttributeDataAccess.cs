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
        var attribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);
        _context.SimAttributes.Remove(attribute!);
        _context.SaveChanges();
    }

    public bool InUseByOther(Guid attributeId)
    {
        return _context.Invocations
                .Any(invocation => invocation.Parameters
                .Any(parameter => parameter.TypeId == attributeId));
    }

    public bool ExistAttributeById(Guid attributeId)
    {
        return _context.SimAttributes
            .Any(a => a.Id == attributeId);
    }

    public bool ExistAttributeName(Guid classId, string attributeName)
    {
        return _context.SimAttributes
            .Any(a => a.RelatedClassId == classId && a.Name == attributeName);
    }

    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute)
    {
        var existingAttribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);

        existingAttribute.Name = attribute.Name;
        existingAttribute.TypeId = attribute.TypeId;
        existingAttribute.Type = attribute.Type;
        existingAttribute.Privacity = attribute.Privacity;
        existingAttribute.RelatedClass = attribute.RelatedClass;
        existingAttribute.RelatedClassId = existingAttribute.RelatedClass.Id;

        _context.SimAttributes.Update(existingAttribute);
        _context.SaveChanges();

        return existingAttribute;
    }
}
