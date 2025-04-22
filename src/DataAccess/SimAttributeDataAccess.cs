using DataAccess.Context;
using DataAccess.CustomExceptions;
using Domain;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class SimAttributeDataAccess(SimulatorDbContext context) : ISimAttributeDataAccess
{
    private readonly SimulatorDbContext _context = context;
    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute)
    {
        try
        {
            attribute.RelatedClassId = classId;
            _context.SimAttributes.Add(attribute);
            _context.SaveChanges();

            return attribute;
        }
        catch(DbUpdateException ex)
        {
            throw new DataAccessException("Data base problem", ex);
        }
    }

    public void DeleteAttribute(Guid attributeId)
    {
        try
        {
            var attribute = _context.SimAttributes.FirstOrDefault(a => a.Id == attributeId);
            _context.SimAttributes.Remove(attribute!);
            _context.SaveChanges();
        }
        catch(Exception ex)
        {
            throw new DataAccessException("Data base problem", ex);
        }
    }

    public bool InUseByOther(Guid attributeId)
    {
        throw new NotImplementedException();
    }

    public bool ExistAttributeById(Guid attributeId)
    {
        try
        {
            return _context.SimAttributes
                .Any(a => a.Id == attributeId);
        }
        catch(Exception ex)
        {
            throw new DataAccessException("Data base problem", ex);
        }
    }

    public bool ExistAttributeName(Guid classId, string attributeName)
    {
        try
        {
            return _context.SimAttributes
                .Any(a => a.RelatedClassId == classId && a.Name == attributeName);
        }
        catch(Exception ex)
        {
            throw new DataAccessException("Data base problem", ex);
        }
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
