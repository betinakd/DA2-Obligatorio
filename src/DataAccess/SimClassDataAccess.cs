using System.Diagnostics.CodeAnalysis;
using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;
[ExcludeFromCodeCoverage]
public class SimClassDataAccess(SimulatorDbContext context) : ISimClassDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public void CreateSimClass(SimClass simClass)
    {
        throw new NotImplementedException();
    }

    public void DeleteSimClass(Guid id)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void UpdateSimClass(SimClass simClass)
    {
        throw new NotImplementedException();
    }
}
