using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassImplementController(ISimClassAdapter simClass) : ControllerBase
{
    private readonly ISimClassAdapter _simClassAdapter = simClass;

    [HttpPost("{id}/implements")]
    public IActionResult ImplementInterface(Guid id, [FromBody] ImplementRequest implementRequest)
    {
        var response = _simClassAdapter.ImplementInterface(id, implementRequest);
        return CreatedAtRoute("GetReferenceClass", new { id = response.SimClass.Id }, response);
    }
}
