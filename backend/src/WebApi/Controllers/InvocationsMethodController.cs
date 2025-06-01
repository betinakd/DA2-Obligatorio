using IAdapter;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/invocations")]
public class InvocationsMethodController(IMethodAdapter invocationAdapter) : ControllerBase
{
    private readonly IMethodAdapter _invocationAdapter = invocationAdapter;

    [HttpGet("{id}", Name = "GetInvocationById")]
    public IActionResult GetInvocation(Guid id)
    {
        var result = _invocationAdapter.GetInvocation(id);
        return Ok(result);
    }
}
