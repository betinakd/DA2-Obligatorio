using IBussinesLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Responses;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class SimClassController(ISimClassService userService) : ControllerBase
{
    private readonly ISimClassService _userService = userService;

    [HttpGet]
    public IActionResult GetAllSimClass()
    {
        return Ok(_userService.GetAllSimClass().Select(x => new SimClassResponse(x)).ToList());
    }
}
