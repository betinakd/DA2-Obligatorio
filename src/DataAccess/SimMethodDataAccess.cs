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
        _context.Invocations.Add(newInvocation);
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
            result = result || methodC.Equals(method);
        }

        return result;
    }

    public bool ExistVariableById(Guid id)
    {
        return _context.LocalVariables.Any(v => v.Id == id);
    }

    public Invocation GetInvocationById(Guid id)
    {
        return _context.Invocations.FirstOrDefault(i => i.Id == id);
    }

    public SimMethod GetMethodById(Guid id)
    {
        return _context.SimMethods.FirstOrDefault(m => m.Id == id);
    }

    public Parameter GetParameterById(Guid id)
    {
        return _context.Parameters.FirstOrDefault(p => p.Id == id);
    }

    public LocalVariable GetVariableById(Guid id)
    {
        return _context.LocalVariables.FirstOrDefault(v => v.Id == id);
    }

    public bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter)
    {
        return _context.Parameters.Any(p => p.RelatedMethodId == methodId && p.Name == parameter.Name);
    }

    public bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable)
    {
        return _context.LocalVariables.Any(v => v.RelatedMethodId == methodId && v.Name == localVariable.Name);
    }
}
