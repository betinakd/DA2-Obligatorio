using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using IDataAccess;

namespace BusinessLogic;

public class NamespaceService(INamespaceDataAccess namespaceDataAccess) : INamespaceService
{
    private readonly INamespaceDataAccess _namespaceDataAccess = namespaceDataAccess;

    public SimNamespace CreateNamespace(SimNamespace simNamespace)
    {
        if(string.IsNullOrEmpty(simNamespace.Name))
        {
            throw new InvalidAttributeLogic("Namespace name cannot be null or empty.");
        }

        if(simNamespace.BaseNamespaceId != Guid.Empty && !_namespaceDataAccess.NamespaceExistsById(simNamespace.BaseNamespaceId))
        {
            throw new NonExistentValueLogic("Base namespace does not exist.");
        }

        if(simNamespace.BaseNamespaceId == Guid.Empty)
        {
            simNamespace.BaseNamespaceId = null;
        }

        if(_namespaceDataAccess.NamespaceExistsByName(simNamespace.Name))
        {
            throw new InvalidAttributeLogic("Namespace with this name already exists at this lavel.");
        }

        _namespaceDataAccess.CreateNamespace(simNamespace);
        return simNamespace;
    }

    public SimNamespace GetNamespaceById(Guid? id)
    {
        if(id == null)
        {
            throw new InvalidAttributeLogic("Namespace can't be empty.");
        }

        var result = _namespaceDataAccess.GetNamespaceById(id.Value);
        if(result == null)
        {
            throw new NonExistentValueLogic($"Namespace with {id} ID name does not exist.");
        }

        return result;
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
