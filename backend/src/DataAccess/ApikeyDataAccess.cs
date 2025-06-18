using IDataAccess;

namespace DataAccess;

public class ApikeyDataAccess(SimulatorDbContext context) : IApikeyDataAccess
{
    private readonly SimulatorDbContext _context = context;
    public bool ApiKeyExists(Guid keyValue)
    {
        var exists = _context.ApiKeys
            .Any(a => a.KeyValue == keyValue);
        return exists;
    }
}
