using Domain;

namespace IBusinessLogic;

public interface INamespaceService
{
    public SimNamespace CreateNamespace(SimNamespace simNamespace);
    public SimNamespace GetNamespaceById(Guid? id);
    public bool NameAlreadyInNamespace_Validation(Guid? id, string className);
    public List<SimNamespace> GetAllNamespaces();
}
