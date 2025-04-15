using IAdapter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/parameters")]
public class ParameterMethodController(IMethodAdapter methodAdapter) : ControllerBase
{
    private readonly IMethodAdapter _simMethodAdapter = methodAdapter;

    [HttpGet("{id}", Name = "GetParameterById")]
    public IActionResult GetParameter(Guid id)
    {
        return Ok(_simMethodAdapter.GetParameter(id));
    }
}
