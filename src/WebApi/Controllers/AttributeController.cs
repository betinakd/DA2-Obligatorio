using IAdapter;
using Microsoft.AspNetCore.Mvc;
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
        var result = _simAttributeAdapter.DeleteAttribute(id);
        return Ok(result);
    }
}