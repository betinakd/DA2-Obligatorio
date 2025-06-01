using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

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
    public IActionResult CreateSimClass([FromBody] SimClassRequestCreate newClass)
    {
        var createdClass = _simClassAdapter.CreateSimClass(newClass);

        return CreatedAtAction(nameof(GetInfoClass), new { classId = createdClass.SimClass.Id }, createdClass);
    }

    [HttpPut]
    public IActionResult UpdateSimClass([FromBody] SimClassRequestUpdate updateClass)
    {
        var simClassResponse = _simClassAdapter.UpdateSimClass(updateClass, updateClass.IdClass);
        return Ok(simClassResponse);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSimClass(Guid id)
    {
        _simClassAdapter.DeleteSimClass(id);
        return NoContent();
    }

    [HttpGet("{classId}", Name = "GetReferenceClass")]
    public IActionResult GetInfoClass([FromRoute] Guid classId)
    {
        var simClassResponse = _simClassAdapter.GetSimClassInfo(classId);
        return Ok(simClassResponse);
    }
}
