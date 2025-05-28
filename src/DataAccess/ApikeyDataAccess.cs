using DataAccess.Context;
using IDataAccess;

namespace DataAccess;

public class ApikeyDataAccess(SimulatorDbContext context) : IApikeyDataAccess
{
    private readonly SimulatorDbContext _context = context;
    public bool ApiKeyExists(Guid keyValue)
    {
        var a = _context;
        return true;

        // throw new NotImplementedException();
    }
}
