using System.Diagnostics.CodeAnalysis;
using DataAccess.Context;
using Domain;
using IDataAccess;

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

    [ExcludeFromCodeCoverage]
    public bool ExistsMethodInClass(Guid idClass, SimMethod method)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public bool ExistVariableById(Guid id)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public Invocation GetInvocationById(Guid id)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public SimMethod GetMethodById(Guid id)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public Parameter GetParameterById(Guid id)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public LocalVariable GetVariableById(Guid id)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable)
    {
        throw new NotImplementedException();
    }
}
