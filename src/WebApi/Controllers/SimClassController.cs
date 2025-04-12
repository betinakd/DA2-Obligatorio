using IAdapter;
using Microsoft.AspNetCore.Mvc;

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
}