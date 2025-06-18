using Adapter.Helpers;
using Domain;
using IAdapter;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using Models.Request;
using Models.Response;

namespace Adapter;

public class NamespaceAdapter(INamespaceService namespaceService, ISimClassService simClassService) : INamespaceAdapter
{
    private readonly INamespaceService _namespaceService = namespaceService;
    private readonly ISimClassService _simClassService = simClassService;
    public CreatedNamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest)
    {
        try
        {
            var simNamespace = new SimNamespace
            {
                Name = namespaceRequest.Name,
                BaseNamespaceId = namespaceRequest.BaseNamespaceId
            };
            var newNamespace = _namespaceService.CreateNamespace(simNamespace);

            var response = new CreatedNamespaceResponse() { NamespaceResponse = new NamespaceResponse { Id = newNamespace.Id, Name = newNamespace.Name, BaseNamespaceId = newNamespace.BaseNamespaceId }, Message = "Namespace created successfully" };

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

    public NamespaceResponse GetNamespaceById(Guid id)
    {
        try
        {
            var simNamespace = _namespaceService.GetNamespaceById(id);
            var classes = _simClassService.GetClassesOfNamespaces(id);
            var response = new NamespaceResponse
            {
                Id = simNamespace.Id,
                Name = simNamespace.Name,
                BaseNamespaceId = simNamespace.BaseNamespaceId,
                Elements = classes.Select(SimClassResponseMapper.MapToSimClassResponse).ToList()
            };

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

    public List<NamespaceResponse> GetAllNamespaces()
    {
        return _namespaceService.GetAllNamespaces()
            .Select(NamespaceResponseMapper.MapToNamespaceResponse)
            .ToList();
    }
}
