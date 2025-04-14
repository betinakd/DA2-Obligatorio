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

    [HttpGet("{id}")]
    public IActionResult GetMethod(Guid id)
    {
        return Ok(_simMethodAdapter.GetMethod(id));
    }

    [HttpPost]
    public IActionResult CreateMethod([FromBody] MethodRequest methodRequest)
    {
        var response = _simMethodAdapter.CreateMethod(methodRequest);
        return CreatedAtAction(nameof(GetMethod), new { id = response.Id }, response);
    }
}
