using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using IDataAccess;
using Models.Request;

namespace BusinessLogic;

public class NamespaceService(INamespaceDataAccess namespaceDataAccess) : INamespaceService
{
    private readonly INamespaceDataAccess _namespaceDataAccess = namespaceDataAccess;

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
        if(id == null)
        {
            throw new InvalidAttributeLogic("Namespace can't be empty.");
        }

        return _namespaceDataAccess.GetNamespaceById(id.Value);
    }

    public bool NameAlreadyInNamespace_Validation(Guid? id, string className)
    {
        try
        {
            var namespaceExists = _namespaceDataAccess.NamespaceExistsById(id);

            var elements = GetNamespaceById(id).Elements;
            return elements.Any(e => e.Name == className);
        }
        catch(InvalidAttributeLogic)
        {
            throw new NonExistentValueLogic($"Namespace with ID {id} does not exist.");
        }
    }

    public List<SimNamespace> GetAllNamespaces()
    {
        return _namespaceDataAccess.GetAllNamespaces();
    }
}
