using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
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

    [HttpPost("{id}/variables")]
    public IActionResult CreateVariables([FromBody] VariableRequest variable, Guid id)
    {
        var response = _simMethodAdapter.CreateVariable(id, variable);
        return CreatedAtRoute("GetVariableById", new { id = response?.Variable?.Id }, response);
    }
}
