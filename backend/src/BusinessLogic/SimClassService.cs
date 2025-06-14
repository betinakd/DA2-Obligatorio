using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using IBusinessLogic;
using IDataAccess;

namespace BusinessLogic;

public class SimClassService(ISimClassDataAccess simClassDA, ISimAttributeDataAccess simAttributeDA, IExecutionDataAccess executionDataAccess, INamespaceService namespaceService) : ISimClassService
{
    private readonly ISimClassDataAccess _simClassDA = simClassDA;
    private readonly ISimAttributeDataAccess _simAttributeDA = simAttributeDA;
    private readonly IExecutionDataAccess _executionDataAccess = executionDataAccess;
    private readonly INamespaceService _namespaceService = namespaceService;
    public SimClass CreateSimClass(string name, SimAccesibility simAccesibility, Guid baseClassId, Guid? namespaceId)
    {
        if(_simClassDA.ExistSimClassName(name))
        {
            throw new InUseValueLogic("SimClass name already exists.");
        }

        if(!_simClassDA.ExistSimClassById(baseClassId))
        {
            throw new NonExistentValueLogic("Base class not found.");
        }

        var nameSpaceSim = _namespaceService.GetNamespaceById(namespaceId);

        if(nameSpaceSim == null)
        {
            throw new NonExistentValueLogic("Namespace not found.");
        }

        try
        {
            var baseClass = _simClassDA.GetSimClassById(baseClassId);
            var simClass = new SimClass()
            {
                Id = Guid.NewGuid(),
                Name = name,
                State = simAccesibility,
                BaseClass = baseClass,
                BaseClassId = baseClass.Id,
                NamespaceId = namespaceId,
                Namespace = nameSpaceSim
            };
            _simClassDA.CreateSimClass(simClass);
            return simClass;
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeLogic(ex.Message);
        }
    }

    public void DeleteSimClass(Guid id)
    {
        if(!_simClassDA.ExistSimClassById(id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        InUseByOther(id);

        _simClassDA.DeleteSimClass(id);
    }

    public IList<SimClass> GetAllSimClasses()
    {
        return _simClassDA.GetAllSimClasses();
    }

    public SimClass GetSimClassById(Guid id)
    {
        if(_simClassDA.ExistSimClassById(id))
        {
            return _simClassDA.GetSimClassById(id);
        }
        else
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }
    }

    public SimClass UpdateSimClass(SimClass simClass)
    {
        var simNamespace = _namespaceService.GetNamespaceById(simClass.NamespaceId);
        if(simNamespace == null)
        {
            throw new NonExistentValueLogic("Namespace not found.");
        }

        if(!_simClassDA.ExistSimClassById(simClass.Id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        if(_simClassDA.HasCyclicDependency(simClass.Id, simClass.BaseClassId))
        {
            throw new InvalidAttributeLogic("Cyclic dependency detected in class inheritance.");
        }

        InUseByOther(simClass.Id);

        simClass.Namespace = simNamespace;
        _simClassDA.UpdateSimClass(simClass);
        return simClass;
    }

    public bool InUseByOther(Guid simClassiId)
    {
        var simClass = _simClassDA.GetSimClassById(simClassiId);

        if(_simClassDA.InUseByOther(simClassiId))
        {
            throw new InUseValueLogic("SimClass is in use as type or baseClass in others entities and cannot be updated.");
        }

        foreach(var attribute in simClass.Attributes)
        {
            if(_simAttributeDA.InUseByOther(attribute.Id))
            {
                throw new InUseValueLogic("An attribute is in used as reference by an invocation and cannot be updated.");
            }
        }

        foreach(var method in simClass.Methods)
        {
            if(_executionDataAccess.MethodIsInUseByInheritingInvocations(method.Id))
            {
                throw new InUseValueLogic("Method is in use by invocations and cannot be updated.");
            }
        }

        return true;
    }

    public void ClassInheritAttribute(Guid idClass, Guid idAttribute)
    {
        if(!_simClassDA.ClassInheritAttribute(idClass, idAttribute))
        {
            throw new NonExistentValueLogic("Attribute not reacheable from method.");
        }
    }

    public void ValidPolymorphism(SimClass baseClass, SimClass derivedClass)
    {
        if(derivedClass.State == SimAccesibility.Interface)
        {
            throw new InvalidAttributeLogic("Polymorphic inheritance is not allowed when the derived type is an interface.");
        }

        if(derivedClass.State == SimAccesibility.Abstract)
        {
            throw new InvalidAttributeLogic("Polymorphic inheritance is not allowed when the base type is abstract.");
        }

        if(!_simClassDA.IsClassBaseOfOrSameAs(baseClass, derivedClass))
        {
            throw new InvalidAttributeLogic("Base class is not base of derived class.");
        }
    }

    public List<SimClass> GetClassesOfNamespaces(Guid id)
    {
        if(_namespaceService.GetNamespaceById(id) == null)
        {
            throw new NonExistentValueLogic("Namespace not found.");
        }

        try
        {
            var allClasses = _simClassDA.GetAllSimClasses();
            var classesInNamespace = allClasses.Where(c => c.NamespaceId == id).ToList();
            return classesInNamespace;
        }
        catch(InvalidAttributeLogic e)
        {
            throw new InvalidAttributeLogic(e.Message);
        }
    }

    public SimClass ImplementInterface(SimClass simClassToUpdate)
    {
        return _simClassDA.ImplementInterface(simClassToUpdate);
    }

    public bool SimClassImplementsInterface(Guid classId, Guid interfaceId)
    {
        if(!_simClassDA.ExistSimClassById(classId))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        if(!_simClassDA.ExistSimClassById(interfaceId))
        {
            throw new NonExistentValueLogic("Interface not found.");
        }

        if(_simClassDA.SimClassImplementsInterface(classId, interfaceId))
        {
            throw new InvalidAttributeLogic("SimClass already implements this interface.");
        }

        return false;
    }
}
