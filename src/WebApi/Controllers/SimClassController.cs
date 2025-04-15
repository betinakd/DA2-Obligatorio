using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/classes")]
public class SimClassController(ISimClassAdapter simClassAdapter) : ControllerBase
{
    private readonly ISimClassAdapter _simClassAdapter = simClassAdapter;

    [HttpGet]
    public IActionResult GetAllSimClass()
    {
        return Ok(_simClassAdapter.GetAllSimClasses().ToList());
    }

    [HttpPost]
    public IActionResult CreateSimClass([FromBody] SimClassRequest newClass)
    {
        var simClassResponse = _simClassAdapter.CreateSimClass(newClass);

        return CreatedAtAction(nameof(CreateSimClass), new { id = simClassResponse.Id }, simClassResponse);
    }

    [HttpPut]
    public IActionResult UpdateSimClass([FromBody] UpdateSimClassRequest updateClass)
    {
        var simClassResponse = _simClassAdapter.UpdateSimClass(updateClass);
        return Ok(simClassResponse);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSimClass(Guid id)
    {
        _simClassAdapter.DeleteSimClass(id);
        return NoContent();
    }

    [HttpGet("{classId}")]
    public IActionResult GetInfoClass([FromRoute] Guid classId)
    {
        var simClassResponse = _simClassAdapter.GetSimClassInfo(classId);
        return Ok(simClassResponse);
    }
}
