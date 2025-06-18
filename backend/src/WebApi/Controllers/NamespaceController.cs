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
        return CreatedAtRoute("GetNamespaceById", new { id = response.NamespaceResponse.Id }, response);
    }

    [HttpGet("{id}", Name = "GetNamespaceById")]
    public IActionResult GetNamespaceById(Guid id)
    {
        return Ok(_namespaceAdapter.GetNamespaceById(id));
    }

    [HttpGet]
    public IActionResult GetAllNamespaces()
    {
        var namespaces = _namespaceAdapter.GetAllNamespaces();
        return Ok(namespaces);
    }
}
