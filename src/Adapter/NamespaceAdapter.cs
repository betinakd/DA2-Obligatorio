using Adapter.Exceptions;
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

    public NamespaceResponse GetNamespaceById(Guid id)
    {
        try
        {
            var @namespace = _namespaceService.GetNamespaceById(id);
            var classes = _simClassService.GetClassesOfNamespaces(id);
            var response = new NamespaceResponse
            {
                Id = @namespace.Id,
                Name = @namespace.Name,
                BaseNamespaceId = @namespace.BaseNamespaceId,
                BaseNamespaceName =
                    @namespace.BaseNamespaceId != null
                        ? _namespaceService.GetNamespaceById(@namespace.BaseNamespaceId.Value).Name
                        : null,
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
            .Select(n => new NamespaceResponse
            {
                Id = n.Id,
                Name = n.Name,
                BaseNamespaceId = n.BaseNamespaceId,
                BaseNamespaceName = n.BaseNamespaceId != null
                    ? _namespaceService.GetNamespaceById(n.BaseNamespaceId.Value).Name
                    : null,
                Elements = MapClassesToResponses(_simClassService.GetClassesOfNamespaces(n.Id))
            }).ToList();
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
