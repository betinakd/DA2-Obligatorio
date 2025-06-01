using Models.Request;
using Models.Response;

namespace IAdapter;

public interface INamespaceAdapter
{
    NamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest);
    NamespaceResponse GetNamespaceById(Guid id);
    List<NamespaceResponse> GetAllNamespaces();
}
