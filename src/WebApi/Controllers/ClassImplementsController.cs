using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassImplementsController(ISimClassAdapter simClassAdapter) : ControllerBase
{
    private readonly ISimClassAdapter _simClassAdapter = simClassAdapter;

    [HttpPost("{id}/implements")]
    public IActionResult AddInterfaceToClass(Guid id, [FromBody] InterfaceRequestUpdate classRequest)
    {
        var response = _simClassAdapter.AddInterface(id, classRequest);
        return CreatedAtRoute("GetSimClass", new { id = id }, response);
    }
}
