using Domain;
using Domain.Enums;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using IDataAccess;

namespace BusinessLogic;

public class SimMethodService(ISimMethodDataAccess simMethodDA, ISimClassDataAccess simClassDA, IExecutionDataAccess executionDA) : IMethodService
{
    private readonly ISimMethodDataAccess _simMethodDA = simMethodDA;
    private readonly ISimClassDataAccess _simClassDA = simClassDA;
    private readonly IExecutionDataAccess _executionDA = executionDA;

    public Invocation AddInvocation(Guid idMethod, Invocation newInvocation)
    {
        if(!_simClassDA.ExistSimClassById(newInvocation.Signature.ReturnTypeId))
        {
            throw new NonExistentValueLogic("ReturnType Class does not exist.");
        }

        if(!_simMethodDA.ExistMethodById(idMethod))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        var method = _simMethodDA.GetMethodById(idMethod);

        if(method.Accesibility == SimAccesibility.Interface)
        {
            throw new InvalidAttributeLogic("Interface methods cannot have invocations.");
        }

        if(method.IsStatic &&
           (newInvocation.Reference.GetReferenceTypeDescription() != "Static" &&
            newInvocation.Reference.GetReferenceTypeDescription() != "StaticAttribute"))
        {
            throw new InvalidAttributeLogic("Static methods cannot have non-static invocations.");
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

        var simClass = _simClassDA.GetSimClassById(idClass);

        if(simClass.State == SimAccesibility.Abstract || method.Accesibility == SimAccesibility.Interface)
        {
            if(_simClassDA.InUseByOther(idClass))
            {
                throw new InUseValueLogic("Cannot add a method in an abstract class or interface in use by other entities.");
            }
        }

        if(_simMethodDA.ExistsMethodInClass(idClass, method))
        {
            throw new InUseValueLogic("Method with same firm is already in the specified class.");
        }

        if(method.Accesibility == SimAccesibility.Abstract && simClass.State != SimAccesibility.Abstract && _simClassDA.InUseByOther(idClass))
        {
            throw new InUseValueLogic("Abstract method cannot be added because the class it is already in use and cannot change to abstract.");
        }

        if(simClass.BaseClassId != null)
        {
            IsValidVirtualOverride(simClass, method);
        }

        if(method.Accesibility == SimAccesibility.Abstract)
        {
            simClass.State = SimAccesibility.Abstract;
            _simClassDA.UpdateSimClass(simClass);
        }

        return _simMethodDA.CreateMethod(idClass, method);
    }

    public Parameter AddMethodParameter(Guid methodId, Parameter parameter)
    {
        var method = _simMethodDA.GetMethodById(methodId);

        if(method == null)
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        if(method.IsVirtual || method.Accesibility == SimAccesibility.Abstract || method.Accesibility == SimAccesibility.Interface)
        {
            throw new InUseValueLogic("Parameter modification is not allowed for virtual, abstract or interface methods after initial creation.");
        }

        if(_simMethodDA.MethodParameterRepeatedValues(methodId, parameter))
        {
            throw new InUseValueLogic("Parameter with that name is already in use.");
        }

        if(_simMethodDA.MethodInUseByInvocations(methodId))
        {
            throw new InUseValueLogic("Cannot add a parameter in a method used by invocations.");
        }

        return _simMethodDA.AddMethodParameter(methodId, parameter);
    }

    public void DeleteMethod(Guid id)
    {
        if(!_simMethodDA.ExistMethodById(id))
        {
            throw new NonExistentValueLogic("Method does not exist.");
        }

        var method = _simMethodDA.GetMethodById(id);
        if((method.IsVirtual && !method.IsOverride) || method.Accesibility == SimAccesibility.Abstract || method.Accesibility == SimAccesibility.Interface)
        {
            throw new InUseValueLogic("Cannot delete a virtual, abstract or interface method.");
        }

        if(_simMethodDA.MethodIsInUse(id))
        {
            throw new InUseValueLogic("Method cannot be deleted because it is in use by parameters, local variables or invocations.");
        }

        if(_simMethodDA.MethodInUseByInvocations(id))
        {
            throw new InUseValueLogic("Method cannot be deleted because it is in use by inheriting classes.");
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

    public void SignatureStaticExistsInClass(SimClass staticClass, Guid invoksMethod, Signature signature)
    {
        var method = GetMethodById(invoksMethod);
        var relatedClass = method.RelatedClass;
        var methodMatchingSignature = staticClass.Methods
            .FirstOrDefault(m => m.MatchSignature(signature) && m.IsStatic && m.Privacity != SimPrivacity.Private);
        if(methodMatchingSignature == null)
        {
            throw new NonExistentValueLogic("No static method with matching signature found in the class.");
        }
    }

    public void ValidateStaticAttributeAccessibility(SimAttribute staticAttribute, Guid methodId)
    {
        var method = GetMethodById(methodId);
        var callingClass = method.RelatedClass;

        var attributeOwnerClass = staticAttribute.RelatedClass;

        switch(staticAttribute.Privacity)
        {
            case SimPrivacity.Private:
                if(callingClass.Id != attributeOwnerClass.Id)
                {
                    throw new InvalidAttributeLogic("Cannot access private static attribute from a different class.");
                }

                break;

            case SimPrivacity.Protected:
                if(callingClass.Id != attributeOwnerClass.Id &&
                    !_simClassDA.IsClassBaseOfOrSameAs(attributeOwnerClass, callingClass))
                {
                    throw new InvalidAttributeLogic("Cannot access protected static attribute from a non-derived class.");
                }

                break;

            case SimPrivacity.Public:
                break;
        }
    }

    public void MethodInheritsAttribute(SimMethod method, SimAttribute attribute)
    {
        foreach(var parameter in method.Parameters)
        {
            if(_simClassDA.ClassInheritAttribute(parameter.ReferenceId, attribute.Id))
            {
                return;
            }
        }

        foreach(var localVariable in method.LocalVariables)
        {
            if(_simClassDA.ClassInheritAttribute(localVariable.ReferenceId, attribute.Id))
            {
                return;
            }
        }

        if(_simClassDA.ClassInheritAttribute(method.RelatedClass.Id, attribute.Id))
        {
            return;
        }

        throw new InvalidAttributeLogic("Method cannot access the specified attribute.");
    }

    public void IsValidVirtualOverride(SimClass simClass, SimMethod method)
    {
        if(simClass.BaseClassId == null)
        {
            throw new NonExistentValueLogic("Cannot override a method in a class without a base class.");
        }

        if(!_executionDA.CanOverrideFromBaseClass((Guid)simClass.BaseClassId, method) &&
           !_executionDA.CanOverrideFromImplementedInterfaces(simClass, method))
        {
            throw new InUseValueLogic("Method cannot be overridden because no virtual or abstract method found in base classes.");
        }

        if(_executionDA.FindSealedMethodInHierarchyFromBaseClass((Guid)simClass.BaseClassId, method) != null)
        {
            throw new InUseValueLogic("Method cannot be overridden because a sealed method with the same signature was found in base classes.");
        }
    }
}
