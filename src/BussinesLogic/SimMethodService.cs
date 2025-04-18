using System.Diagnostics.CodeAnalysis;
using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class SimMethodService(ISimMethodDataAccess simMethodDA, ISimClassDataAccess simClassDA) : IMethodService
{
    private readonly ISimMethodDataAccess _simMethodDA = simMethodDA;
    private readonly ISimClassDataAccess _simClassDA = simClassDA;

    public Invocation AddInvocation(Guid idMethod, Invocation newInvocation)
    {
        if(!_simMethodDA.ExistMethodById(idMethod))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        return _simMethodDA.CreateInvocation(idMethod, newInvocation);
    }

    [ExcludeFromCodeCoverage]
    public SimAttribute AddLocalVariable(Guid methodId, LocalVariable localVariable)
    {
        throw new NotImplementedException();
    }

    public SimMethod AddMethod(Guid id, SimMethod method)
    {
        if(!_simClassDA.ExistSimClassById(id))
        {
            throw new NonExistentValueLogic("Sim class does not exist.");
        }

        return null;
    }

    [ExcludeFromCodeCoverage]
    public SimAttribute AddMethodParameter(Guid methodId, Parameter parameter)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public SimMethod CreateClassMethod(string? name, string? returnType, string? accessModifier, bool? isStatic, bool? isAbstract, Guid? classId)
    {
        throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    public void DeleteMethod(Guid id)
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
}
