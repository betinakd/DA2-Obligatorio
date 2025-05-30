using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using IDataAccess;
using Models.Request;

namespace BusinessLogic;

public class NamespaceService(INamespaceDataAccess namespaceDataAccess, ISimClassDataAccess simClassDataAccess) : INamespaceService
{
    private readonly INamespaceDataAccess _namespaceDataAccess = namespaceDataAccess;
    private readonly ISimClassDataAccess _simClassDataAccess = simClassDataAccess;

    public SimNamespace CreateNamespace(NamespaceRequest simNamespace)
    {
        if(simNamespace.Name == string.Empty)
        {
            throw new InvalidAttributeLogic("Namespace name cannot be empty.");
        }

        if(simNamespace.BaseNamespaceId != null && !_namespaceDataAccess.NamespaceExistsById(simNamespace.BaseNamespaceId.Value))
        {
            throw new NonExistentValueLogic("Base namespace does not exist.");
        }

        var namespaces = _namespaceDataAccess.GetAllNamespaces();
        if(namespaces.Any(ns => ns.Name == simNamespace.Name && ns.BaseNamespaceId == simNamespace.BaseNamespaceId))
        {
            throw new InvalidAttributeLogic("Namespace with this name already exists at this lavel.");
        }

        var newNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = simNamespace.Name,
            BaseNamespaceId = simNamespace.BaseNamespaceId
        };
        _namespaceDataAccess.CreateNamespace(newNamespace);
        return newNamespace;
    }

    public SimNamespace GetNamespaceById(Guid? id)
    {
        return _namespaceDataAccess.GetNamespaceById(id.Value);
    }

    public string AddClassInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        throw new NotImplementedException();
    }

    public string AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        throw new NotImplementedException();
    }
}
