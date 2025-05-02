using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

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
    public IActionResult CreateVariables([FromBody] VariablesRequestCreateClass variable, Guid id)
    {
        var response = _simMethodAdapter.CreateVariable(id, variable);
        return CreatedAtRoute("GetVariableById", new { id = response?.Variable?.Id }, response);
    }

    [HttpPost("{id}/parameters")]
    public IActionResult CreateParameter([FromBody] ParameterRequestCreateClass parameter, Guid id)
    {
        var response = _simMethodAdapter.CreateParameter(id, parameter);
        return CreatedAtRoute("GetParameterById", new { id = response?.Parameter?.Id }, response);
    }

    [HttpPost("{id}/invocations")]
    public IActionResult CreateInvocation([FromBody] InvocationRequestCreateClass invocationRequest, Guid id)
    {
        var response = _simMethodAdapter.CreateInvocation(id, invocationRequest);
        return CreatedAtRoute("GetInvocationId", new { id = response?.InvocationResponse?.Id }, response);
    }
}
