using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/attributes")]
public class AttributeController(IAttributeAdapter simClassAdapter) : ControllerBase
{
    private readonly IAttributeAdapter _simAttributeAdapter = simClassAdapter;

    [HttpDelete("{id}")]
    public IActionResult DeleteAttribute(Guid id)
    {
        _simAttributeAdapter.DeleteAttribute(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAttribute(Guid id, [FromBody] AttributeRequest attribute)
    {
        var result = _simAttributeAdapter.UpdateAttribute(id, attribute);
        return Ok(result);
    }
}
