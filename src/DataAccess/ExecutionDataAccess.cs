using DataAccess.Context;
using Domain;
using Domain.Enums;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;
public class ExecutionDataAccess(SimulatorDbContext context) : IExecutionDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public SimMethod FindMethodInHierarchy(SimClass simClass, Signature signature)
    {
        if(simClass == null)
        {
            return null;
        }

        var methods = _context.SimMethods
            .Include(m => m.Parameters)
                .ThenInclude(p => p.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => i.Reference)
            .Include(m => m.Invocations)
                .ThenInclude(i => i.Signature)
                    .ThenInclude(s => s.Parameters)
                        .ThenInclude(p => p.Type)
            .Where(m => m.RelatedClassId == simClass.Id && m.Name == signature.Name &&
                  (m.Privacity == SimPrivacity.Public || m.Privacity == SimPrivacity.Protected))
            .ToList();

        var method = methods.FirstOrDefault(m => m.MatchSignature(signature));

        if(method != null)
        {
            return method;
        }

        if(!simClass.BaseClassId.HasValue)
        {
            return null;
        }

        var baseClass = _context.SimClasses
            .Include(c => c.BaseClass)
            .FirstOrDefault(c => c.Id == simClass.BaseClassId.Value);

        if(baseClass == null)
        {
            return null;
        }

        return FindMethodInHierarchy(baseClass, signature);
    }
}
