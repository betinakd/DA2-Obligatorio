using IAdapter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/invocations")]
public class InvocationsMethodController(IMethodInvocationAdapter invocationAdapter) : ControllerBase
{
    private readonly IMethodInvocationAdapter _invocationAdapter = invocationAdapter;

    [HttpGet("{id}", Name = "GetInvocationById")]
    public IActionResult GetInvocation(Guid id)
    {
        var result = _invocationAdapter.GetInvocation(id);
        return Ok(result);
    }
}
