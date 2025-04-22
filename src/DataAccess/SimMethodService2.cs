using DataAccess.Context;
using Domain;
using IDataAccess;

namespace DataAccess;

public class SimMethodDataAccess2(SimulatorDbContext context) : ISimMethodDataAccess
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
        throw new NotImplementedException();
    }

    public SimMethod CreateMethod(Guid idClass, SimMethod method)
    {
        throw new NotImplementedException();
    }

    public void DeleteMethod(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistInvocationById(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistMethodById(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistParameter(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistsMethodInClass(Guid idClass, SimMethod method)
    {
        throw new NotImplementedException();
    }

    public bool ExistVariableById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Invocation GetInvocationById(Guid id)
    {
        throw new NotImplementedException();
    }

    public SimMethod GetMethodById(Guid id)
    {
        throw new NotImplementedException();
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
