using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;

public class SimClassDataAccess(SimulatorDbContext context) : ISimClassDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public void CreateSimClass(SimClass simClass)
    {
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();
    }

    public void DeleteSimClass(Guid id)
    {
        var simClass = _context.SimClasses.FirstOrDefault(c => c.Id == id);
        if(simClass != null)
        {
            _context.SimClasses.Remove(simClass);
            _context.SaveChanges();
        }
    }

    public bool ExistSimClassById(Guid id)
    {
        return _context.SimClasses.Any(c => c.Id == id);
    }

    public bool ExistSimClassName(string name)
    {
        throw new NotImplementedException();
    }

    public IList<SimClass> GetAllSimClasses()
    {
        throw new NotImplementedException();
    }

    public SimClass GetSimClassById(Guid id)
    {
        return _context.SimClasses.FirstOrDefault(c => c.Id == id);
    }

    public bool InUseByOther(Guid id)
    {
        var baseClass = _context.SimClasses.Any(c => c.BaseClassId == id);
        var attribute = _context.SimAttributes.Any(c => c.RelatedClassId == id || c.TypeId == id);
        var method = _context.SimMethods.Any(c => c.RelatedClassId == id);
        var parameter = _context.Parameters.Any(c => c.TypeId == id);

        return baseClass || attribute || method || parameter;
    }

    public void UpdateSimClass(SimClass simClass)
    {
        throw new NotImplementedException();
    }
}
