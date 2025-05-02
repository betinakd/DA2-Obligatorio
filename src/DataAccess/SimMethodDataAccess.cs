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
        _context.LocalVariables.Add(localVariable);
        _context.SaveChanges();
        return localVariable;
    }

    public Parameter AddMethodParameter(Guid methodId, Parameter parameter)
    {
        _context.Parameters.Add(parameter);
        _context.SaveChanges();
        return parameter;
    }

    public Invocation CreateInvocation(Guid idMethod, Invocation newInvocation)
    {
        var method = _context.SimMethods
            .Include(m => m.Invocations)
            .FirstOrDefault(m => m.Id == idMethod);

        if(method == null)
        {
            throw new ArgumentException($"Method with ID {idMethod} not found");
        }

        newInvocation.Index = method.Invocations.Count;
        newInvocation.RelatedMethodId = method.Id;

        _context.Invocations.Add(newInvocation);

        method.Invocations.Add(newInvocation);

        _context.SaveChanges();

        return newInvocation;
    }

    public SimMethod CreateMethod(Guid idClass, SimMethod method)
    {
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
            .FirstOrDefault();
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

    public bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter)
    {
        return _context.Parameters.Any(p => p.RelatedMethodId == methodId && p.Name.ToLower() == parameter.Name.ToLower());
    }

    public bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable)
    {
        return _context.LocalVariables.Any(v => v.RelatedMethodId == methodId && v.Name.ToLower() == localVariable.Name.ToLower());
    }
}
