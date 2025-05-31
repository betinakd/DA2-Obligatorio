using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;

public class NamespaceDataAccess(SimulatorDbContext context) : INamespaceDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public void CreateNamespace(SimNamespace simNamespace)
    {
        throw new NotImplementedException();
    }

    public bool NamespaceExistsById(Guid? id)
    {
        throw new NotImplementedException();
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
