using Models.Request;
using Models.Response;

namespace IAdapter;

public interface INamespaceAdapter
{
    NamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest);
    string AddClassInNamespace(Guid id, NamespaceElementAdd_Request request);
    string AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request);
}
