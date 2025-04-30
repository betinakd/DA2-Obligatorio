using DataAccess.Context;
using Domain;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

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
        var simClass = _context.SimClasses
            .Include(c => c.Attributes)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Parameters)
            .Include(c => c.Methods)
                .ThenInclude(m => m.LocalVariables)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => i.Reference)
            .FirstOrDefault(c => c.Id == id);

        if(simClass == null)
        {
            return;
        }

        var referenceThis = _context.References.OfType<ReferenceThis>()
            .Where(r => r.ReferenceId == id).ToList();
        _context.References.RemoveRange(referenceThis);

        var referenceBase = _context.References.OfType<ReferenceBase>()
            .Where(r => r.ReferenceId == id).ToList();
        _context.References.RemoveRange(referenceBase);

        foreach(var method in simClass.Methods)
        {
            foreach(var inv in method.Invocations)
            {
                if(inv.Reference != null)
                {
                    _context.References.Remove(inv.Reference);
                }

                if(inv.Signature != null)
                {
                    _context.Signatures.Remove(inv.Signature);
                }
            }

            _context.Invocations.RemoveRange(method.Invocations);
            _context.Parameters.RemoveRange(method.Parameters);
            _context.LocalVariables.RemoveRange(method.LocalVariables);
        }

        _context.SimMethods.RemoveRange(simClass.Methods);

        _context.SimAttributes.RemoveRange(simClass.Attributes);

        _context.SimClasses.Remove(simClass);
        _context.SaveChanges();
    }

    public bool ExistSimClassById(Guid id)
    {
        return _context.SimClasses.Any(c => c.Id == id);
    }

    public bool ExistSimClassName(string name)
    {
        return _context.SimClasses.Any(c => c.Name.ToLower() == name.ToLower());
    }

    public IList<SimClass> GetAllSimClasses()
    {
        return _context.SimClasses.ToList();
    }

    public SimClass GetSimClassById(Guid id)
    {
        var simClass = _context.SimClasses
            .Include(c => c.BaseClass)
            .Include(c => c.Attributes)
                .ThenInclude(a => a.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Parameters)
                    .ThenInclude(p => p.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.LocalVariables)
                    .ThenInclude(v => v.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => i.Signature)
                        .ThenInclude(s => s.Parameters)
                            .ThenInclude(p => p.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => (i.Reference as ReferenceParameter).Reference)
                        .ThenInclude(p => p.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => (i.Reference as ReferenceVariable).Reference)
                        .ThenInclude(v => v.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => (i.Reference as ReferenceAttribute).Reference)
                        .ThenInclude(a => a.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => (i.Reference as ReferenceBase).Reference)
                        .ThenInclude(c => c.BaseClass)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => (i.Reference as ReferenceThis).Reference)
            .FirstOrDefault(c => c.Id == id);

        return simClass;
    }

    public bool InUseByOther(Guid id)
    {
        var baseClass = _context.SimClasses.Any(c => c.BaseClassId == id);
        var typeAttribute = _context.SimAttributes.Any(a => a.TypeId == id && a.RelatedClassId != id);
        var typeParameter = _context.Parameters.Any(p => p.TypeId == id);
        var typeLocalVar = _context.LocalVariables.Any(v => v.TypeId == id);
        var parameter = _context.Parameters.Any(p => p.TypeId == id);
        var method = _context.SimMethods.Any(m => m.ReturnTypeId == id);

        var referenceThis = _context.References
            .OfType<ReferenceThis>()
            .Any(r => r.ReferenceId == id);

        var referenceBase = _context.References
            .OfType<ReferenceBase>()
            .Any(r => r.ReferenceId == id);

        return baseClass || typeAttribute || typeParameter ||
               typeLocalVar || parameter || method ||
               referenceThis || referenceBase;
    }

    public void UpdateSimClass(SimClass simClass)
    {
        var existingSimClass = _context.SimClasses.FirstOrDefault(c => c.Id == simClass.Id);
        if(existingSimClass != null)
        {
            existingSimClass.Name = simClass.Name;
            existingSimClass.BaseClassId = simClass.BaseClassId;
            existingSimClass.State = simClass.State;

            _context.SaveChanges();
        }
    }
}
