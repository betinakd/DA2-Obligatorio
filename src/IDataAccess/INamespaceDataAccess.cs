using Domain;

namespace IDataAccess;

public interface INamespaceDataAccess
{
    public void CreateNamespace(SimNamespace simNamespace);
    public bool NamespaceExistsById(Guid? id);
    public SimNamespace GetNamespaceById(Guid? id);
    public List<SimNamespace> GetAllNamespaces();
}
