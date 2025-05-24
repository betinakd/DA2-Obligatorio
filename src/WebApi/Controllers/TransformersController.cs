using IAdapter;
using IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Transformers.Abstractions;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/transformers")]
public class TransformersController(ITransformerService transformerService, IExecutionAdapter executionAdapter) : ControllerBase
{
    private readonly ITransformerService _transformerService = transformerService;
    private readonly IExecutionAdapter _executionAdapter = executionAdapter;

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

    [HttpPost("execute")]
    public IActionResult ExecuteWithTransform(
        [FromBody] MethodExecutionRequest request,
        [FromQuery] string transformerId = null)
    {
        var result = _executionAdapter.ExecuteMethodWithTransform(request, transformerId);
        return Ok(result);
    }

    [HttpPost("transform")]
    public IActionResult TransformExecution(
    [FromBody] TransformRequest request,
    [FromQuery] string transformerId = null)
    {
        if(request == null)
        {
            return BadRequest("Se requiere un resultado de ejecución");
        }

        var result = _transformerService.TransformExecution(request.ExecutionResult, transformerId);
        return Ok(result);
    }
}
