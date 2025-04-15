using IAdapter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/variables")]
public class VariableMethodController(IMethodAdapter methodAdapter) : ControllerBase
{
    private readonly IMethodAdapter _simMethodAdapter = methodAdapter;

    [HttpGet("{id}")]
    public IActionResult GetVariable(Guid id)
    {
        return Ok(_simMethodAdapter.GetVariable(id));
    }
}
