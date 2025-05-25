using DataAccess.Context;
using Domain;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class SimMethodDataAccess(SimulatorDbContext context) : ISimMethodDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public LocalVariable AddLocalVariable(Guid methodId, LocalVariable localVariable)
    {
        var method = _context.SimMethods
            .Include(m => m.LocalVariables)
            .FirstOrDefault(m => m.Id == methodId);

        localVariable.RelatedMethodId = methodId;

        var updatedVariables = method.LocalVariables;
        updatedVariables.Add(localVariable);
        method.LocalVariables = updatedVariables;

        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();

        return localVariable;
    }

    public Parameter AddMethodParameter(Guid methodId, Parameter parameter)
    {
        var method = _context.SimMethods
            .Include(m => m.Parameters)
            .FirstOrDefault(m => m.Id == methodId);

        parameter.Index = method.Parameters.Count;
        parameter.RelatedMethodId = method.Id;

        var updatedParameter = method.Parameters;
        updatedParameter.Add(parameter);

        method.Parameters = updatedParameter;

        _context.Parameters.Add(parameter);
        _context.SaveChanges();

        return parameter;
    }

    public Invocation CreateInvocation(Guid idMethod, Invocation newInvocation)
    {
        var method = _context.SimMethods
            .Include(m => m.Invocations)
            .FirstOrDefault(m => m.Id == idMethod);

        newInvocation.Index = method.Invocations.Count;
        newInvocation.RelatedMethodId = method.Id;

        var updatedInvocations = method.Invocations;
        updatedInvocations.Add(newInvocation);
        method.Invocations = updatedInvocations;

        _context.Invocations.Add(newInvocation);
        _context.SaveChanges();

        return newInvocation;
    }

    public SimMethod CreateMethod(Guid idClass, SimMethod method)
    {
        var simClass = _context.SimClasses
            .Include(c => c.Methods)
            .FirstOrDefault(c => c.Id == idClass);

        method.RelatedClassId = idClass;
        var updatedMethods = simClass.Methods;
        updatedMethods.Add(method);
        simClass.Methods = updatedMethods;

        _context.SimMethods.Add(method);
        _context.SaveChanges();
        return method;
    }

    public void DeleteMethod(Guid id)
    {
        var method = _context.SimMethods.Find(id);
        if(method != null)
        {
            _context.SimMethods.Remove(method);
            _context.SaveChanges();
        }
    }

    public bool ExistInvocationById(Guid id)
    {
        return _context.Invocations.Any(i => i.Id == id);
    }

    public bool ExistMethodById(Guid id)
    {
        return _context.SimMethods.Any(m => m.Id == id);
    }

    public bool ExistParameter(Guid id)
    {
        return _context.Parameters.Any(p => p.Id == id);
    }

    public bool ExistsMethodInClass(Guid idClass, SimMethod method)
    {
        var methodsClass = _context.SimMethods
            .Include(m => m.Parameters)
                .ThenInclude(p => p.Type)
            .Where(a => a.RelatedClassId == idClass)
            .ToList();
        var result = false;
        foreach(var methodC in methodsClass)
        {
            if(methodC.Parameters != null)
            {
                methodC.Parameters = methodC.Parameters.OrderBy(p => p.Index).ToList();
            }

            if(methodC.Equals(method))
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public bool ExistVariableById(Guid id)
    {
        return _context.LocalVariables.Any(v => v.Id == id);
    }

    public Invocation GetInvocationById(Guid id)
    {
        var invocation = _context.Invocations
            .Where(i => i.Id == id)
            .Include(i => i.Reference)
            .Include(i => i.Signature)
                .ThenInclude(s => s.Parameters)
                    .ThenInclude(p => p.Type)
            .Include(i => i.RelatedMethod)
            .FirstOrDefault();

        if(invocation != null)
        {
            if(invocation.Signature?.Parameters != null)
            {
                invocation.Signature.Parameters = invocation.Signature.Parameters
                    .OrderBy(p => p.Index)
                    .ToList();
            }

            switch(invocation.Reference)
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

        return invocation;
    }

    public SimMethod GetMethodById(Guid id)
    {
        var method = _context.SimMethods
            .Where(m => m.Id == id)
            .Include(a => a.RelatedClass)
            .Include(b => b.ReturnType)
            .Include(m => m.Parameters)
                .ThenInclude(p => p.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => i.Signature)
                    .ThenInclude(s => s.Parameters)
                        .ThenInclude(p => p.Type)
            .Include(m => m.Invocations)
                .ThenInclude(i => i.Reference)
            .FirstOrDefault();

        if(method != null)
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

        return method;
    }

    public Parameter GetParameterById(Guid id)
    {
        var parameter = _context.Parameters
            .Where(p => p.Id == id)
            .Include(p => p.Type)
            .Include(p => p.RelatedMethod)
            .FirstOrDefault();
        return parameter;
    }

    public LocalVariable GetVariableById(Guid id)
    {
        var variable = _context.LocalVariables
            .Where(v => v.Id == id)
            .Include(v => v.Type)
            .Include(v => v.RelatedMethod)
            .FirstOrDefault();
        return variable;
    }

    public bool MethodIsInUse(Guid id)
    {
        return _context.LocalVariables.Any(v => v.RelatedMethodId == id) ||
               _context.Parameters.Any(p => p.RelatedMethodId == id) ||
               _context.Invocations.Any(i => i.RelatedMethodId == id);
    }

    public bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter)
    {
        return _context.Parameters.Any(p => p.RelatedMethodId == methodId && p.Name.ToLower() == parameter.Name.ToLower());
    }

    public bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable)
    {
        return _context.LocalVariables.Any(v => v.RelatedMethodId == methodId && v.Name.ToLower() == localVariable.Name.ToLower());
    }
}
