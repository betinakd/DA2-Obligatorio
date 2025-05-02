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
                .ThenInclude(i => (i.Reference as ReferenceParameter).Reference)
                    .ThenInclude(p => p.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => (i.Reference as ReferenceVariable).Reference)
                    .ThenInclude(v => v.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => (i.Reference as ReferenceAttribute).Reference)
                    .ThenInclude(a => a.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => (i.Reference as ReferenceBase).Reference)
                    .ThenInclude(c => c.BaseClass)
            .Include(m => m.Invocations)
                .ThenInclude(i => (i.Reference as ReferenceThis).Reference)
            .Include(m => m.Invocations)
                .ThenInclude(i => i.Signature)
                    .ThenInclude(s => s.Parameters)
                        .ThenInclude(p => p.Type)
                            .Where(m =>
                                m.RelatedClassId == simClass.Id &&
                                m.Name == signature.Name &&
                                (level == 0 || m.Privacity == SimPrivacity.Public
                                || m.Privacity == SimPrivacity.Protected)).ToList();

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
                .ThenInclude(m => m.Invocations)
                    .ThenInclude(a => a.Signature)
                        .ThenInclude(r => r.Parameters)
            .FirstOrDefault(c => c.Id == simMethod.RelatedClassId);

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

    public bool MethodIsOverridingSealed(Guid idClass, SimMethod method)
    {
        var ownerClass = _context.SimClasses
            .Include(c => c.Methods)
            .ThenInclude(m => m.Parameters)
            .ThenInclude(p => p.Type)
            .FirstOrDefault(c => c.Id == idClass);

        if(ownerClass == null)
        {
            return false;
        }

        foreach(var simMethod in ownerClass.Methods)
        {
            if(simMethod.Accesibility == SimAccesibility.Sealed && simMethod.Equals(method))
            {
                return true;
            }
        }

        if(!ownerClass.BaseClassId.HasValue)
        {
            return false;
        }

        return MethodIsOverridingSealed(ownerClass.BaseClassId.Value, method);
    }

    public void SaveExecutionLog(ExecutionLog executionLog)
    {
        _context.ExecutionLogs.Add(executionLog);
        _context.SaveChanges();
    }
}
