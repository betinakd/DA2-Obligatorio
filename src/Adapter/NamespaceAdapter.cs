using IAdapter;
using IBusinessLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class NamespaceAdapter(INamespaceService namespaceService) : INamespaceAdapter
{
    private readonly INamespaceService _namespaceService = namespaceService;
    public NamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest)
    {
        var newNamespace = _namespaceService.CreateNamespace(namespaceRequest);
        var response = new NamespaceResponse
        {
            Id = newNamespace.Id,
            Name = newNamespace.Name,
        };
        if(namespaceRequest.BaseNamespaceId != null)
        {
            var baseName = _namespaceService.GetNamespaceById(namespaceRequest.BaseNamespaceId);
            response.BaseNamespaceId = newNamespace.BaseNamespaceId;
            response.BaseNamespaceName = baseName.Name;
        }

        return response;
    }

    public string AddClassInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        return _namespaceService.AddClassInNamespace(id, request);
    }

    public string AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        return _namespaceService.AddInterfaceInNamespace(id, request);
    }
}
