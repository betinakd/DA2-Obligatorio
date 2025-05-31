using Domain;

namespace IDataAccess;

public interface INamespaceDataAccess
{
    public void CreateNamespace(SimNamespace simNamespace);
    public bool NamespaceExistsById(Guid? id);
    public SimNamespace GetNamespaceById(Guid? id);
    public void AddClassInNamespace(Guid namespaceId, Guid classId);
    public void AddInterfaceInNamespace(Guid namespaceId, Guid interfaceId);
    public List<SimNamespace> GetAllNamespaces();
}
