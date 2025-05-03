using DataAccess.Context;
using Domain;
using Domain.Enums;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;
public class ExecutionDataAccess(SimulatorDbContext context) : IExecutionDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public SimMethod FindMethodInHierarchy(SimClass simClass, Signature signature, int level = 0)
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
            .Where(m =>
                m.RelatedClassId == simClass.Id &&
                m.Name == signature.Name &&
                (level == 0 || m.Privacity == SimPrivacity.Public
                || m.Privacity == SimPrivacity.Protected))
            .ToList();

        foreach(var m in methods)
        {
            m.Parameters = m.Parameters.OrderBy(p => p.Index).ToList();

            m.Invocations = m.Invocations.OrderBy(i => i.Index).ToList();

            foreach(var inv in m.Invocations)
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

        return FindMethodInHierarchy(baseClass, signature, level + 1);
    }

    public List<SimClass> GetAllInheritingClasses(Guid baseClassId)
    {
        var directInheritors = _context.SimClasses
            .Include(c => c.Methods)
            .ThenInclude(m => m.Invocations)
            .ThenInclude(a => a.Signature)
            .ThenInclude(r => r.Parameters)
            .Where(c => c.BaseClassId == baseClassId)
            .ToList();

        var allInheritors = new List<SimClass>(directInheritors);

        foreach(var inheritor in directInheritors)
        {
            allInheritors.AddRange(GetAllInheritingClasses(inheritor.Id));
        }

        return allInheritors;
    }

    public bool MethodIsInUseByInheritingInvocations(Guid methodId)
    {
        var simMethod = _context.SimMethods.FirstOrDefault(m => m.Id == methodId);
        var ownerSimClass = _context.SimClasses.FirstOrDefault(c => c.Id == simMethod.RelatedClassId);
        var inheritingClasses = GetAllInheritingClasses(simMethod.RelatedClassId);

        var ownerClass = _context.SimClasses
            .Include(c => c.Methods)
                .ThenInclude(m => m.Parameters)
                    .ThenInclude(p => p.Type)
            .Include(c => c.Methods)
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(i => i.Signature)
                        .ThenInclude(s => s.Parameters)
                            .ThenInclude(p => p.Type)
            .FirstOrDefault(c => c.Id == simMethod.RelatedClassId);

        if(ownerClass != null)
        {
            foreach(var method in ownerClass.Methods)
            {
                method.Parameters = method.Parameters.OrderBy(p => p.Index).ToList();

                method.Invocations = method.Invocations.OrderBy(i => i.Index).ToList();

                foreach(var invocation in method.Invocations)
                {
                    if(invocation.Signature?.Parameters != null)
                    {
                        invocation.Signature.Parameters = invocation.Signature.Parameters
                            .OrderBy(p => p.Index)
                            .ToList();
                    }
                }
            }
        }

        if(ownerClass != null)
        {
            foreach(var method in ownerClass.Methods)
            {
                var matchingInvocations = method.Invocations
                    .Where(invocation => simMethod.MatchSignature(invocation.Signature))
                    .ToList();

                if(matchingInvocations.Any())
                {
                    return true;
                }
            }
        }

        foreach(var simClass in inheritingClasses)
        {
            foreach(var method in simClass.Methods)
            {
                var matchingInvocations = method.Invocations
                    .Where(invocation => simMethod.MatchSignature(invocation.Signature) &&
                    (simMethod.Privacity == SimPrivacity.Public || simMethod.Privacity == SimPrivacity.Protected))
                    .ToList();

                if(matchingInvocations.Any())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool ClassInheritAttribute(Guid classId, Guid attributeId, int level = 0)
    {
        var currentClass = _context.SimClasses
            .Include(c => c.Attributes)
            .FirstOrDefault(c => c.Id == classId);

        if(currentClass == null)
        {
            return false;
        }

        if(currentClass.Attributes.Any(a => a.Id == attributeId) && level == 0)
        {
            return true;
        }

        if(currentClass.Attributes.Any(a => a.Id == attributeId && (a.Privacity == SimPrivacity.Public || a.Privacity == SimPrivacity.Protected)) && level != 0)
        {
            return true;
        }

        if(currentClass.BaseClassId.HasValue)
        {
            return ClassInheritAttribute(currentClass.BaseClassId.Value, attributeId, level + 1);
        }

        return false;
    }

    public bool MethodIsOverridingSealed(Guid idClass, SimMethod methodSim)
    {
        var ownerClass = _context.SimClasses
            .Include(c => c.Methods)
                .ThenInclude(m => m.Parameters)
                    .ThenInclude(p => p.Type)
            .FirstOrDefault(c => c.Id == idClass);

        if(ownerClass != null)
        {
            foreach(var method in ownerClass.Methods)
            {
                method.Parameters = method.Parameters.OrderBy(p => p.Index).ToList();
            }
        }

        if(ownerClass == null)
        {
            return false;
        }

        foreach(var simMethod in ownerClass.Methods)
        {
            if(simMethod.Accesibility == SimAccesibility.Sealed && simMethod.Equals(methodSim))
            {
                return true;
            }
        }

        if(!ownerClass.BaseClassId.HasValue)
        {
            return false;
        }

        return MethodIsOverridingSealed(ownerClass.BaseClassId.Value, methodSim);
    }

    public void SaveExecutionLog(ExecutionLog executionLog)
    {
        _context.ExecutionLogs.Add(executionLog);
        _context.SaveChanges();
    }
}
