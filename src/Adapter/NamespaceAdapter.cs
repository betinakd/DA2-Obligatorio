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
            BaseNamespaceId = namespaceRequest.BaseNamespaceId,
            Classes = [],
            Interfaces = []
        };

        return response;
    }
}
