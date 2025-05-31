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
    public SimClass CreateSimClass(string name, SimAccesibility simAccesibility, Guid baseClassId, Guid? namespaceId = null)
    {
        if(_simClassDA.ExistSimClassName(name))
        {
            throw new InUseValueLogic("SimClass name already exists.");
        }

        if(!_simClassDA.ExistSimClassById(baseClassId))
        {
            throw new NonExistentValueLogic("Base class not found.");
        }

        if(_namespaceService.NameAlreadyInNamespace_Validation(namespaceId, name))
        {
            throw new InUseValueLogic("Class name already exists in the namespace.");
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
        if(!_simClassDA.ExistSimClassById(simClass.Id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        InUseByOther(simClass.Id);

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

    public SimClass AddInterface(Guid id, Guid interfaceId)
    {
        if(!_simClassDA.ExistSimClassById(id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        if(!_simClassDA.ExistSimClassById(interfaceId))
        {
            throw new NonExistentValueLogic("Interface not found.");
        }

        var simClass = _simClassDA.GetSimClassById(id);
        var interfaceToAdd = _simClassDA.GetSimClassById(interfaceId);

        if(simClass.Implements.Contains(interfaceToAdd))
        {
            throw new InUseValueLogic("Interface already added.");
        }

        var implementsToUpdate = simClass.Implements;
        implementsToUpdate.Add(interfaceToAdd);
        simClass.SetImplements(implementsToUpdate);
        UpdateSimClass(simClass);

        return simClass;
    }

    public void ClassInheritAttribute(Guid idClass, Guid idAttribute)
    {
        if(!_simClassDA.ClassInheritAttribute(idClass, idAttribute))
        {
            throw new NonExistentValueLogic("Attribute not reacheable from method.");
        }
    }
}
