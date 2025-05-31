using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;

public class NamespaceDataAccess(SimulatorDbContext context) : INamespaceDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public void CreateNamespace(SimNamespace simNamespace)
    {
        _context.SimNamespaces.Add(simNamespace);
        _context.SaveChanges();
    }

    public bool NamespaceExistsById(Guid? id)
    {
        return _context.SimNamespaces.Any(c => c.Id == id);
    }

    public SimNamespace GetNamespaceById(Guid? id)
    {
        throw new NotImplementedException();
    }

    public List<SimNamespace> GetAllNamespaces()
    {
        return _context.SimNamespaces.ToList();
    }
}
