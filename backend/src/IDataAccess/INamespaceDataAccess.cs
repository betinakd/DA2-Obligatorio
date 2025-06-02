using Domain;

namespace IDataAccess;

public interface INamespaceDataAccess
{
    public void CreateNamespace(SimNamespace simNamespace);
    public bool NamespaceExistsById(Guid? id);
    public bool NamespaceExistsByName(string name);
    public SimNamespace GetNamespaceById(Guid? id);
    public List<SimNamespace> GetAllNamespaces();
}
