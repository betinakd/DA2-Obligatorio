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

    public LocalVariable AddLocalVariable(Guid methodId, LocalVariable localVariable)
    {
        if(!_simMethodDA.ExistMethodById(methodId))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        if(_simMethodDA.MethodVariableRepeatedValues(methodId, localVariable))
        {
            throw new InUseValueLogic("Local variable with that name is already in use.");
        }

        return _simMethodDA.AddLocalVariable(methodId, localVariable);
    }

    public SimMethod AddMethod(Guid idClass, SimMethod method)
    {
        if(!_simClassDA.ExistSimClassById(idClass))
        {
            throw new NonExistentValueLogic("Sim class does not exist.");
        }

        if(_simMethodDA.ExistsMethodInClass(idClass, method))
        {
            throw new InUseValueLogic("Method with same firm is already in the specified class.");
        }

        return _simMethodDA.CreateMethod(idClass, method);
    }

    [ExcludeFromCodeCoverage]
    public SimAttribute AddMethodParameter(Guid methodId, Parameter parameter)
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
