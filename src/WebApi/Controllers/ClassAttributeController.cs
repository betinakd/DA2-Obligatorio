using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassAttributeController(IAttributeAdapter simClassAdapter) : ControllerBase
{
    private readonly IAttributeAdapter _simAttributeAdapter = simClassAdapter;

    [HttpPost("{id}/attributes")]
    public IActionResult CreateAttribute(Guid id, [FromBody] AttributeRequest attribute)
    {
        var result = _simAttributeAdapter.CreateAttribute(id, attribute);
        return Ok(result);
    }
}
