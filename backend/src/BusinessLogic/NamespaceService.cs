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

        if(simNamespace.BaseNamespaceId != null && simNamespace.BaseNamespaceId != Guid.Empty)
        {
            if(!_namespaceDataAccess.NamespaceExistsById(simNamespace.BaseNamespaceId))
            {
                throw new NonExistentValueLogic("Base namespace does not exist.");
            }
        }

        if(_namespaceDataAccess.NamespaceExistsByName(simNamespace.Name))
        {
            throw new InvalidAttributeLogic("Namespace with this name already exists.");
        }

        _namespaceDataAccess.CreateNamespace(simNamespace);
        return simNamespace;
    }

    public SimNamespace GetNamespaceById(Guid? id)
    {
        var result = _namespaceDataAccess.GetNamespaceById(id.Value);
        if(result == null)
        {
            throw new NonExistentValueLogic($"Namespace with {id} ID name does not exist.");
        }

        return result;
    }

    public List<SimNamespace> GetAllNamespaces()
    {
        return _namespaceDataAccess.GetAllNamespaces();
    }
}
