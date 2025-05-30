using Adapter.Exceptions;
using BusinessLogic.Exceptions;
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
        try
        {
            var newNamespace = _namespaceService.CreateNamespace(namespaceRequest);
            var response = new NamespaceResponse { Id = newNamespace.Id, Name = newNamespace.Name, };
            if(namespaceRequest.BaseNamespaceId != null)
            {
                var baseName = _namespaceService.GetNamespaceById(namespaceRequest.BaseNamespaceId);
                response.BaseNamespaceId = newNamespace.BaseNamespaceId;
                response.BaseNamespaceName = baseName.Name;
            }

            return response;
        }
        catch(InvalidAttributeLogic e)
        {
            throw new InvalidAttributeAdapter(e.Message);
        }
        catch(NonExistentValueLogic e)
        {
            throw new NonExistentValueAdapter(e.Message);
        }
    }

    public string AddClassInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        try
        {
            return _namespaceService.AddClassInNamespace(id, request);
        }
        catch(NonExistentValueLogic e)
        {
            throw new NonExistentValueAdapter(e.Message);
        }
    }

    public string AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        try
        {
            return _namespaceService.AddInterfaceInNamespace(id, request);
        }
        catch(NonExistentValueLogic e)
        {
            throw new NonExistentValueAdapter(e.Message);
        }
    }
}
