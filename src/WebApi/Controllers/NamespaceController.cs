using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/namespaces")]
public class NamespaceController(INamespaceAdapter namespaceAdapter) : ControllerBase
{
    private readonly INamespaceAdapter _namespaceAdapter = namespaceAdapter;
    [HttpPost]
    public IActionResult CreateNamespace([FromBody] NamespaceRequest request)
    {
        var response = _namespaceAdapter.CreateNamespace(request);
        return Created("GetNamespaceById", response);
    }

    [HttpPost("{id}/classes")]
    public IActionResult AddClassInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        return Ok(_namespaceAdapter.AddClassInNamespace(id, request));
    }

    [HttpPost("{id}/classes")]
    public IActionResult AddInterfaceInNamespace(Guid id, NamespaceElementAdd_Request request)
    {
        return Ok(_namespaceAdapter.AddInterfaceInNamespace(id, request));
    }
}
