using Domain;
using Models.Request;

namespace IBusinessLogic;

public interface INamespaceService
{
    public SimNamespace CreateNamespace(NamespaceRequest simNamespace);
    public SimNamespace GetNamespaceById(Guid? id);
    public bool NameAlreadyInNamespace_Validation(Guid? id, string className);

    // bool AddElementInNamespace(Guid id, NamespaceElementAdd_Request request);
    // string AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request);
}
