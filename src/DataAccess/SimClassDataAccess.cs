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
            .Include(c => c.Implements)
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

        var implementsToRemove = _context.SimClasses
            .Where(c => c.Id == id)
            .SelectMany(c => c.Implements)
            .ToList();

        foreach(var impl in implementsToRemove)
        {
            _context.Entry(simClass).Collection("SimClassImplements").EntityEntry
                .State = EntityState.Deleted;
        }

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
        var classes = _context.SimClasses
            .Include(c => c.Attributes)
                .ThenInclude(a => a.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Parameters)
                    .ThenInclude(p => p.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.ReturnType)
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
                    .ThenInclude(i => i.Reference)
            .Include(c => c.BaseClass)
            .Include(c => c.Implements)
                .ThenInclude(i => i.Methods)
                    .ThenInclude(m => m.Parameters)
                        .ThenInclude(p => p.Type)
            .AsSplitQuery()
            .ToList();

        foreach(var simClass in classes)
        {
            simClass.Methods = simClass.Methods.OrderBy(m => m.Name).ToList();

            foreach(var method in simClass.Methods)
            {
                method.Parameters = method.Parameters
                    .OrderBy(p => p.Index)
                    .ToList();

                method.LocalVariables = method.LocalVariables
                    .OrderBy(v => v.Name)
                    .ToList();

                method.Invocations = method.Invocations
                    .OrderBy(i => i.Index)
                    .ToList();

                foreach(var inv in method.Invocations)
                {
                    if(inv.Signature?.Parameters != null)
                    {
                        inv.Signature.Parameters = inv.Signature.Parameters
                            .OrderBy(p => p.Index)
                            .ToList();
                    }

                    switch(inv.Reference)
                    {
                        case ReferenceParameter rp:
                            _context.Entry(rp).Reference(r => r.Reference).Query().Include(p => p.Type).Load();
                            break;
                        case ReferenceVariable rv:
                            _context.Entry(rv).Reference(r => r.Reference).Query().Include(v => v.Type).Load();
                            break;
                        case ReferenceAttribute ra:
                            _context.Entry(ra).Reference(r => r.Reference).Query().Include(a => a.Type).Load();
                            break;
                        case ReferenceBase rb:
                            _context.Entry(rb).Reference(r => r.Reference).Query().Include(c => c.BaseClass).Load();
                            break;
                        case ReferenceThis rt:
                            _context.Entry(rt).Reference(r => r.Reference).Load();
                            break;
                    }
                }
            }
        }

        return classes;
    }

    public SimClass GetSimClassById(Guid id)
    {
        var simClass = _context.SimClasses
            .Where(c => c.Id == id)
            .Include(c => c.BaseClass)
            .Include(c => c.Attributes).ThenInclude(a => a.Type)
            .Include(c => c.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Type)
            .Include(c => c.Methods).ThenInclude(m => m.LocalVariables).ThenInclude(v => v.Type)
            .Include(c => c.Methods).ThenInclude(m => m.Invocations).ThenInclude(i => i.Signature).ThenInclude(s => s.Parameters).ThenInclude(p => p.Type)
            .Include(c => c.Methods).ThenInclude(m => m.Invocations).ThenInclude(i => i.Reference)
            .Include(c => c.Implements).ThenInclude(i => i.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Type)
            .Include(c => c.Methods).ThenInclude(m => m.ReturnType)
            .AsSplitQuery()
            .FirstOrDefault();

        if(simClass == null)
        {
            return null;
        }

        foreach(var method in simClass.Methods)
        {
            method.Invocations = method.Invocations
                .OrderBy(i => i.Index)
                .ToList();

            method.Parameters = method.Parameters
                .OrderBy(p => p.Index)
                .ToList();

            foreach(var inv in method.Invocations)
            {
                if(inv.Signature?.Parameters != null)
                {
                    inv.Signature.Parameters = inv.Signature.Parameters
                        .OrderBy(p => p.Index)
                        .ToList();
                }

                switch(inv.Reference)
                {
                    case ReferenceParameter rp:
                        _context.Entry(rp).Reference(r => r.Reference).Query().Include(p => p.Type).Load();
                        break;
                    case ReferenceVariable rv:
                        _context.Entry(rv).Reference(r => r.Reference).Query().Include(v => v.Type).Load();
                        break;
                    case ReferenceAttribute ra:
                        _context.Entry(ra).Reference(r => r.Reference).Query().Include(a => a.Type).Load();
                        break;
                    case ReferenceBase rb:
                        _context.Entry(rb).Reference(r => r.Reference).Query().Include(c => c.BaseClass).Load();
                        break;
                    case ReferenceThis rt:
                        _context.Entry(rt).Reference(r => r.Reference).Load();
                        break;
                }
            }
        }

        return simClass;
    }

    public bool InUseByOther(Guid id)
    {
        var baseClass = _context.SimClasses.Any(c => c.BaseClassId == id);
        var typeAttribute = _context.SimAttributes.Any(a => a.TypeId == id && a.RelatedClassId != id);
        var typeParameter = _context.Parameters.Any(p => p.TypeId == id);
        var typeLocalVar = _context.LocalVariables.Any(v => v.TypeId == id);
        var parameter = _context.ParameterSignatures.Any(p => p.TypeId == id);
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
        DeleteSimClass(simClass.Id);
        _context.SimClasses.Add(simClass);
        _context.SaveChanges();
    }
}
