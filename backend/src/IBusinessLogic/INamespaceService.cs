using Domain;

namespace IBusinessLogic;

public interface INamespaceService
{
    public SimNamespace CreateNamespace(SimNamespace simNamespace);
    public SimNamespace GetNamespaceById(Guid? id);
    public List<SimNamespace> GetAllNamespaces();
}
