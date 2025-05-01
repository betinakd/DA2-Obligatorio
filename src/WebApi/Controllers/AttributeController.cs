using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

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

    [HttpGet("{id}")]
    public IActionResult GetAttribute(Guid id)
    {
        var attribute = _simAttributeAdapter.GetAttribute(id);
        return Ok(attribute);
    }
}
