using IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/transformers")]
public class TransformersController(ITransformerService transformerService) : ControllerBase
{
    private readonly ITransformerService _transformerService = transformerService;

    [HttpGet]
    public IActionResult GetTransformers()
    {
        return Ok(_transformerService.GetAvailableTransformers());
    }

    [HttpPost("reload")]
    public IActionResult ReloadTransformers()
    {
        _transformerService.LoadTransformers();
        return Ok(new
        {
            message = "Transformadores recargados correctamente",
            transformers = _transformerService.GetAvailableTransformers()
        });
    }
}
