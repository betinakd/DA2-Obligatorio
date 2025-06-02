using Adapter.Exceptions;
using Adapter.Helpers;
using BusinessLogic.Exceptions;
using Domain;
using IAdapter;
using IBusinessLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class NamespaceAdapter(INamespaceService namespaceService, ISimClassService simClassService) : INamespaceAdapter
{
    private readonly INamespaceService _namespaceService = namespaceService;
    private readonly ISimClassService _simClassService = simClassService;
    public NamespaceResponse CreateNamespace(NamespaceRequest namespaceRequest)
    {
        try
        {
            var simNamespace = new SimNamespace
            {
                Name = namespaceRequest.Name,
                BaseNamespaceId = namespaceRequest.BaseNamespaceId != Guid.Empty ? namespaceRequest.BaseNamespaceId : null
            };
            var newNamespace = _namespaceService.CreateNamespace(simNamespace);
            var response = new NamespaceResponse { Id = newNamespace.Id, Name = newNamespace.Name, };
            if(namespaceRequest.BaseNamespaceId != Guid.Empty)
            {
                var baseName = _namespaceService.GetNamespaceById(namespaceRequest.BaseNamespaceId);
                response.BaseNamespaceId = newNamespace.BaseNamespaceId;
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
                Elements = MapClassesToResponses(classes)
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

    private List<SimClassResponse> MapClassesToResponses(List<SimClass> classes)
    {
        if(classes == null || !classes.Any())
        {
            return [];
        }
        else
        {
            return [.. classes.Select(c => new SimClassResponse
            {
                Id = c.Id,
                Name = c.Name,
                State = (Models.Enums.SimModelsAccesibility)c.State,
                IdBaseClass = c.BaseClassId,
            })];
        }
    }
}
