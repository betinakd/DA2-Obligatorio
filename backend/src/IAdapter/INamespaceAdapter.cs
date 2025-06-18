using Models.Request;
using Models.Response;

namespace IAdapter;

public interface INamespaceAdapter
{
    CreatedNamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest);
    NamespaceResponse GetNamespaceById(Guid id);
    List<NamespaceResponse> GetAllNamespaces();
}
