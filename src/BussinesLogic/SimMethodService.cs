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

    public Parameter AddMethodParameter(Guid methodId, Parameter parameter)
    {
        if(!_simMethodDA.ExistMethodById(methodId))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        if(_simMethodDA.MethodParameterRepeatedValues(methodId, parameter))
        {
            throw new InUseValueLogic("Parameter with that name is already in use.");
        }

        return _simMethodDA.AddMethodParameter(methodId, parameter);
    }

    public void DeleteMethod(Guid id)
    {
        if(!_simMethodDA.ExistMethodById(id))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        _simMethodDA.DeleteMethod(id);
    }

    public Invocation GetInvocationById(Guid id)
    {
        if(!_simMethodDA.ExistInvocationById(id))
        {
            throw new NonExistentValueLogic("Invocation does not exist.");
        }

        return _simMethodDA.GetInvocationById(id);
    }

    public SimMethod GetMethodById(Guid id)
    {
        if(!_simMethodDA.ExistMethodById(id))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        return _simMethodDA.GetMethodById(id);
    }

    public Parameter GetParameterById(Guid id)
    {
        if(!_simMethodDA.ExistParameter(id))
        {
            throw new NonExistentValueLogic("Parameter does not exist.");
        }

        return _simMethodDA.GetParameterById(id);
    }

    public LocalVariable GetVariableById(Guid id)
    {
        if(!_simMethodDA.ExistVariableById(id))
        {
            throw new NonExistentValueLogic("Variable does not exist.");
        }

        return _simMethodDA.GetVariableById(id);
    }
}
