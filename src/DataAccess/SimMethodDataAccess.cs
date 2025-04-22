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
        throw new NotImplementedException();
    }

    public LocalVariable GetVariableById(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool MethodParameterRepeatedValues(Guid methodId, Parameter parameter)
    {
        throw new NotImplementedException();
    }

    public bool MethodVariableRepeatedValues(Guid methodId, LocalVariable localVariable)
    {
        throw new NotImplementedException();
    }
}
