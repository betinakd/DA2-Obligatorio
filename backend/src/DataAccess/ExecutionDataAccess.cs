using DataAccess.Context;
using Domain;
using Domain.Enums;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ExecutionDataAccess(SimulatorDbContext context) : IExecutionDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public List<SimClass> GetAllInheritingClasses(Guid baseClassId)
    {
        var directInheritors = GetFilteredClasses(query =>
            query.Where(c => c.BaseClassId == baseClassId));

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
        if(simMethod == null)
        {
            return false;
        }

        var ownerClass = GetFilteredClasses(query =>
            query.Where(c => c.Id == simMethod.RelatedClassId)).FirstOrDefault();

        if(IsMethodUsedInClass(ownerClass, simMethod))
        {
            return true;
        }

        var inheritingClasses = GetAllInheritingClasses(simMethod.RelatedClassId);
        return inheritingClasses.Any(c => IsMethodUsedInInheritingClass(c, simMethod));
    }

    private bool IsMethodUsedInClass(SimClass? simClass, SimMethod simMethod)
    {
        if(simClass == null)
        {
            return false;
        }

        if(simClass.Methods == null)
        {
            return false;
        }

        return simClass.Methods.Any(m =>
            m.Invocations.Any(i => simMethod.MatchSignature(i.Signature)));
    }

    private bool IsMethodUsedInInheritingClass(SimClass simClass, SimMethod simMethod)
    {
        if(simClass?.Methods == null)
        {
            return false;
        }

        var isAccessible = simMethod.Privacity == SimPrivacity.Public ||
                           simMethod.Privacity == SimPrivacity.Protected;

        return simClass.Methods.Any(m =>
            m.Invocations.Any(i =>
                simMethod.MatchSignature(i.Signature) && isAccessible));
    }

    public bool CanOverride(Guid classId, SimMethod methodToOverride)
    {
        if(methodToOverride == null)
        {
            return false;
        }

        if(!methodToOverride.IsOverride)
        {
            return true;
        }

        var currentClass = GetFilteredClasses(query =>
            query.Where(c => c.Id == classId))
            .FirstOrDefault();

        if(currentClass == null || !currentClass.BaseClassId.HasValue)
        {
            return false;
        }

        var baseClassId = currentClass.BaseClassId.Value;
        var baseClass = GetFilteredClasses(query =>
            query.Where(c => c.Id == baseClassId))
            .FirstOrDefault();

        if(baseClass == null)
        {
            return false;
        }

        var baseMethod = baseClass.Methods.FirstOrDefault(m =>
            m.Equals(methodToOverride) &&
            (m.IsVirtual || m.Accesibility == SimAccesibility.Abstract || m.Accesibility == SimAccesibility.Interface) &&
            (m.Privacity == SimPrivacity.Public || m.Privacity == SimPrivacity.Protected));

        if(baseMethod != null)
        {
            return true;
        }

        if(baseClass.BaseClassId.HasValue && baseClass.BaseClassId.Value != baseClassId)
        {
            return CanOverride(baseClass.Id, methodToOverride);
        }

        return false;
    }

    public SimMethod FindSealedMethodInHierarchy(Guid classId, SimMethod methodToCheck)
    {
        var baseClass = GetFilteredClasses(query =>
            query.Where(c => c.Id == classId))
            .FirstOrDefault();

        if(baseClass == null)
        {
            return null;
        }

        if(!methodToCheck.IsVirtual)
        {
            return null;
        }

        var sealedMethod = baseClass.Methods.FirstOrDefault(m =>
            m.Equals(methodToCheck) &&
            m.Accesibility == SimAccesibility.Sealed);

        if(sealedMethod != null)
        {
            return sealedMethod;
        }

        if(baseClass.BaseClassId.HasValue)
        {
            return FindSealedMethodInHierarchy(baseClass.BaseClassId.Value, methodToCheck);
        }

        return null;
    }

    public void SaveExecutionLog(ExecutionLog executionLog)
    {
        _context.ExecutionLogs.Add(executionLog);
        _context.SaveChanges();
    }

    private void OrderMethodElements(SimMethod method)
    {
        method.Parameters = method.Parameters.OrderBy(p => p.Index).ToList();

        method.Invocations = method.Invocations.OrderBy(i => i.Index).ToList();

        foreach(var inv in method.Invocations)
        {
            if(inv.Signature?.Parameters != null)
            {
                inv.Signature.Parameters = inv.Signature.Parameters
                    .OrderBy(p => p.Index)
                    .ToList();
            }
        }
    }

    private void OrderMethodsInClass(SimClass simClass)
    {
        if(simClass?.Methods == null)
        {
            return;
        }

        foreach(var method in simClass.Methods)
        {
            OrderMethodElements(method);
        }
    }

    private void LoadReferenceDetails(Invocation inv)
    {
        if(inv == null)
        {
            return;
        }

        switch(inv.Reference)
        {
            case ReferenceParameter rp:
                _context.Entry(rp).Reference(r => r.Reference).Query().Include(p => p.Reference).Load();
                break;
            case ReferenceVariable rv:
                _context.Entry(rv).Reference(r => r.Reference).Query().Include(v => v.Reference).Include(b => b.Instance).Load();
                break;
            case ReferenceAttribute ra:
                _context.Entry(ra).Reference(r => r.Reference).Query().Include(a => a.Reference).Include(b => b.Instance).Load();
                break;
            case ReferenceBase rb:
                _context.Entry(rb).Reference(r => r.Reference).Query().Include(c => c.BaseClass).Load();
                break;
            case ReferenceThis rt:
                _context.Entry(rt).Reference(r => r.Reference).Load();
                break;
            case ReferenceStaticAttribute rsa:
                _context.Entry(rsa).Reference(r => r.Reference).Query().Include(a => a.Reference).Include(b => b.Instance).Load();
                break;
            case ReferenceStatic rsv:
                _context.Entry(rsv).Reference(r => r.Reference).Load();
                break;
        }
    }

    private List<SimMethod> GetFilteredMethods(Func<IQueryable<SimMethod>, IQueryable<SimMethod>> filter)
    {
        var query = _context.SimMethods
            .Include(m => m.Parameters).ThenInclude(p => p.Reference)
            .Include(m => m.Invocations).ThenInclude(i => i.Reference)
            .Include(m => m.Invocations).ThenInclude(i => i.Signature).ThenInclude(s => s.Parameters).ThenInclude(p => p.Reference)
            .Include(m => m.Invocations).ThenInclude(i => i.Signature).ThenInclude(s => s.Parameters).ThenInclude(p => p.Instance)
            .Include(m => m.Invocations).ThenInclude(i => i.Signature).ThenInclude(s => s.ReturnType)
            .Include(m => m.ReturnType);

        var filteredMethods = filter(query).ToList();

        foreach(var method in filteredMethods)
        {
            OrderMethodElements(method);

            foreach(var inv in method.Invocations)
            {
                LoadReferenceDetails(inv);
            }
        }

        return filteredMethods;
    }

    public List<SimClass> GetFilteredClasses(Func<IQueryable<SimClass>, IQueryable<SimClass>> filter)
    {
        var query = _context.SimClasses
            .Include(c => c.BaseClass)
            .Include(c => c.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Reference)
            .Include(c => c.Methods).ThenInclude(m => m.Invocations).ThenInclude(i => i.Signature)
                .ThenInclude(s => s.Parameters).ThenInclude(p => p.Reference)
            .Include(c => c.Methods).ThenInclude(m => m.Invocations).ThenInclude(i => i.Reference)
            .Include(c => c.Methods).ThenInclude(m => m.ReturnType)
            .AsSplitQuery();

        var filteredClasses = filter(query).ToList();

        foreach(var simClass in filteredClasses)
        {
            OrderMethodsInClass(simClass);

            foreach(var method in simClass.Methods)
            {
                foreach(var inv in method.Invocations)
                {
                    LoadReferenceDetails(inv);
                }
            }
        }

        return filteredClasses;
    }

    public SimMethod FindMethodInHierarchyPublicOrProtected(SimClass simClass, Signature signature, int level = 0)
    {
        if(simClass == null)
        {
            return null;
        }

        var methods = GetFilteredMethods(query => query.Where(m =>
            m.RelatedClassId == simClass.Id &&
            m.Name == signature.Name && !m.IsStatic && (level == 0
            || m.Privacity == SimPrivacity.Public || m.Privacity == SimPrivacity.Protected)));

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

        return baseClass == null ? null : FindMethodInHierarchyPublicOrProtected(baseClass, signature, level + 1);
    }

    public SimMethod? FindOverrideOrReferenceMethod(SimClass instanceClass, SimClass referenceClass, Signature signature)
    {
        SimClass? current = instanceClass;
        while(current != null)
        {
            var methods = GetFilteredMethods(query => query.Where(m =>
                m.RelatedClassId == current.Id &&
                m.Name == signature.Name));

            var overrideMethod = methods.FirstOrDefault(m => m.MatchSignature(signature) && m.IsOverride);
            if(overrideMethod != null)
            {
                return overrideMethod;
            }

            if(!current.BaseClassId.HasValue)
            {
                break;
            }

            current = _context.SimClasses
                .Include(c => c.BaseClass)
                .FirstOrDefault(c => c.Id == current.BaseClassId.Value);
        }

        var referenceMethods = FindMethodInHierarchyPublicOrProtected(referenceClass, signature);

        return referenceMethods;
    }
}
