using IAdapter;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/variables")]
public class VariableMethodController(IMethodAdapter methodAdapter) : ControllerBase
{
    private readonly IMethodAdapter _simMethodAdapter = methodAdapter;

    [HttpGet("{id}", Name = "GetVariableById")]
    public IActionResult GetVariable(Guid id)
    {
        return Ok(_simMethodAdapter.GetVariable(id));
    }
}
