using IAdapter;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/methods")]
public class MethodController(IMethodAdapter methodAdapter) : ControllerBase
{
    private readonly IMethodAdapter _simMethodAdapter = methodAdapter;

    [HttpGet("{id}", Name = "GetMethodId")]
    public IActionResult GetMethod(Guid id)
    {
        return Ok(_simMethodAdapter.GetMethod(id));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMethod(Guid id)
    {
        _simMethodAdapter.DeleteMethod(id);
        return NoContent();
    }
}
