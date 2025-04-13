using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using System;

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
    public IActionResult CreateSimClass([FromBody] SimClassRequest newClass)
    {
        var simClassResponse = _simClassAdapter.CreateSimClass(newClass);
        return Created("Class created successfully.", simClassResponse);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSimClass(Guid id)
    {
        simClassAdapter.DeleteSimClass(id);
        return NoContent();
    }
}
